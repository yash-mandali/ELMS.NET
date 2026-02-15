using collageProject.Data;
using collageProject.Model;
using collageProject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace collageProject.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public UserProfileController(AppDbContext context,
                                      IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // =====================================================
        // 1️⃣ Get Profile By UserId
        // =====================================================
        // GET: api/UserProfile/get/5
        [HttpGet("getProfileById")]
        public async Task<IActionResult> GetProfile([FromQuery] int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid UserId");

            //var profile = await _context.UserProfiles
            //     .AsNoTracking()
            //     .FirstOrDefaultAsync(x => x.UserId == userId);

            var profile = await _context.UserProfiles
    .AsNoTracking()
    .Where(x => x.UserId == userId)
    .Select(x => new UserProfileDto
    {
        ProfileId = x.ProfileId,
        UserId = x.UserId,
        FullName = x.FullName,
        Email = x.Email,
        PhoneNumber = x.PhoneNumber,
        Address = x.Address,
        DateOfBirth = x.DateOfBirth,
        Gender = x.Gender,
        Department = x.Department,
        Role = x.Role,
        ProfileImage = x.ProfileImage,
        CreatedAt = x.CreatedAt,
        UpdatedAt = x.UpdatedAt
    })
    .FirstOrDefaultAsync();


            if (profile == null)
                return NotFound("Profile not found");

            return Ok(profile);
        }


    }
}
