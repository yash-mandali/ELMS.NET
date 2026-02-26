using collageProject.Data;
using collageProject.Model;
using collageProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace collageProject.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public RequestController(AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //[HttpPost("CreateNewRequest")]
        //public async Task<IActionResult> CreateRequest(Requests request)
        //{
        //    var profileExists = await _context.UserProfiles
        //        .AnyAsync(p => p.UserId == request.UserId);

        //    var today = DateOnly.FromDateTime(DateTime.Now);

        //    var maxAllowedDate = today.AddMonths(2);

        //    if (request.FromDate > request.ToDate)
        //    {
        //        return BadRequest(new { Message = "From Date cannot be after To Date." });
        //    }

        //    if (request.FromDate < today)
        //    {
        //        return BadRequest(new { message = "Past date not allowed." });
        //    }

        //    if (request.FromDate > maxAllowedDate)
        //    {
        //        return BadRequest(new
        //        {
        //            message = "You can apply leave only within next 2 months."
        //        });
        //    }

        //    if (!profileExists)
        //    {
        //        return BadRequest(new
        //        {
        //            status = false,
        //            message = "Please complete your profile first."
        //        });
        //    }

        //    var from = request.FromDate.ToDateTime(TimeOnly.MinValue);
        //    var to = request.ToDate.ToDateTime(TimeOnly.MinValue);

        //    //// Base days (inclusive)
        //    //double totalDays = (to - from).Days + 1;

        //    //// Half day logic
        //    //if (request.Session == "Half Day - Morning" || request.Session == "Half Day - Afternoon")
        //    //{
        //    //    totalDays = 0.5;
        //    //}

        //    //request.TotalDays = totalDays;

        //    //-----For already applied---------

        //    bool overlap = await _context.LeaveRequests.AnyAsync(x =>
        //        x.RequestType == request.RequestType &&
        //        x.UserId == request.UserId &&
        //        x.Status != "Rejected" &&
        //        request.FromDate == x.FromDate &&
        //        request.ToDate == x.ToDate);

        //    if (overlap)
        //    {
        //        return BadRequest(new
        //        {
        //            message = "Leave already applied for selected dates."
        //        });
        //    }

        //    _context.LeaveRequests.Add(request);
        //    await _context.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        status = true,
        //        message = "Leave request submitted.",

        //    });
        //}

        [HttpPost("CreateNewRequest")]
        public async Task<IActionResult> CreateRequest(Requests request)
        {
            try
            {
                // Check profile
                var profileExists = await _context.UserProfiles
                    .AnyAsync(p => p.UserId == request.UserId);

                if (!profileExists)
                {
                    return BadRequest(new
                    {
                        message = "Please complete your profile first."
                    });
                }

                // Validate dates
                if (request.FromDate > request.ToDate)
                {
                    return BadRequest(new
                    {
                        message = "From Date cannot be after To Date."
                    });
                }

                var today = DateOnly.FromDateTime(DateTime.Now);

                if (request.FromDate < today)
                {
                    return BadRequest(new
                    {
                        message = "Past date not allowed."
                    });
                }

                if (request.FromDate > today.AddMonths(2))
                {
                    return BadRequest(new
                    {
                        message = "You can apply leave only within next 2 months."
                    });
                }

                // Calculate days
                var from = request.FromDate.ToDateTime(TimeOnly.MinValue);
                var to = request.ToDate.ToDateTime(TimeOnly.MinValue);

                double totalDays = (to - from).Days + 1;

                // Half day logic
                if (request.Session == "Half Day - Morning" ||
                    request.Session == "Half Day - Afternoon")
                {
                    if (request.FromDate != request.ToDate)
                    {
                        return BadRequest(new
                        {
                            message = "Half Day leave must be for same date."
                        });
                    }

                    totalDays = 0.5;
                }

                request.TotalDays = totalDays;

                // Check duplicate
                bool overlap = await _context.LeaveRequests.AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.RequestType == request.RequestType &&
                    x.Status != "Rejected" &&
                    x.FromDate == request.FromDate &&
                    x.ToDate == request.ToDate
                );

                if (overlap)
                {
                    return BadRequest(new
                    {
                        message = "Leave already applied for selected dates."
                    });
                }

                // Save
                _context.LeaveRequests.Add(request);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    status = true,
                    message = "Leave request submitted successfully.",
                    TotalDays = totalDays
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Server error",
                    error = ex.Message
                });
            }
        }

        [HttpGet("AllRequests")]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _context.LeaveRequests
                .Include(x => x.UserProfile)
                .OrderByDescending(x => x.AppliedOn)
                .ToListAsync();

            return Ok(requests);
        }

        [HttpGet("getMyRequests")]
        public async Task<IActionResult> GetMyRequests(int userId)
        {
            var requests = await _context.LeaveRequests
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.AppliedOn)
                .ToListAsync();

            return Ok(requests);
        }

        [HttpPut("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(int requestId, string status)
        {
            var request = await _context.LeaveRequests
                .FirstOrDefaultAsync(x => x.LeaveRequestId == requestId);

            if (request == null)
                return NotFound(new { message = "Request not found." });

            request.Status = status; // Approved / Rejected
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = true,
                message = $"Leave {status} successfully."
            });
        }

        [HttpDelete("CancelLeaveById")]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var request = await _context.LeaveRequests.FindAsync(id);

            if (request == null)
                return NotFound();

            if (request.Status != "Pending")
            {
                return BadRequest(new
                {
                    message = "Only pending requests can be cancelled."
                });
            }

            _context.LeaveRequests.Remove(request);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Leave cancelled."
            });
        }

        [HttpGet("Summary")]
        public async Task<IActionResult> GetSummary(int userId)
        {
            var total = await _context.LeaveRequests
                .CountAsync(x => x.UserId == userId);

            var approved = await _context.LeaveRequests
                .CountAsync(x => x.UserId == userId && x.Status == "Approved");

            var pending = await _context.LeaveRequests
                .CountAsync(x => x.UserId == userId && x.Status == "Pending");

            var rejected = await _context.LeaveRequests
                .CountAsync(x => x.UserId == userId && x.Status == "Rejected");

            return Ok(new
            {
                total,
                approved,
                pending,
                rejected
            });
        }
    }
}
