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

     
        [HttpPost("createProfile")]
        public async Task<IActionResult> CreateProfile([FromBody] UserProfile model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
                .Where(u => u.Id == model.UserId)
                .Select(u => new { u.Email, u.Role })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found");
            }


            // Check if profile already exists
            var exist = await _context.UserProfiles
                .AnyAsync(x => x.UserId == model.UserId);

            if (exist) {
                return BadRequest("Profile already exists");
            }

            model.Email = user.Email;
            model.Role = user.Role;
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = null;
            

            _context.UserProfiles.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile created successfully",
                profileId = model.ProfileId
            });
        }

        [HttpPut("updateProfile")]
        public async Task<IActionResult> UpdateProfile(int id, UpdateUserProfile model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get existing profile
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (profile == null)
                return NotFound("Profile not found");

            // Update allowed fields ONLY
            profile.FullName = model.FullName;
            profile.PhoneNumber = model.PhoneNumber;
            profile.Address = model.Address;
            profile.DateOfBirth = model.DateOfBirth;
            profile.Gender = model.Gender;
            profile.Department = model.Department;
            profile.ProfileImage = model.ProfileImage;
            profile.UpdatedAt = DateTime.UtcNow;

            _context.UserProfiles.Update(profile);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully",
                profileId = profile.ProfileId
            });
        }

        [HttpDelete("deleteProfile")]
        public async Task<IActionResult> DeleteProfile(int userId)
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (profile == null)
                return NotFound("Profile not found");

            _context.UserProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return Ok(new {message = "Profile deleted" });
        }




        //[HttpPut("updateProfile/{userId}")]
        //public async Task<IActionResult> UpdateProfile([FromRoute]int userId, [FromBody] UserProfile model)
        //{
        //    if (model == null)
        //        return BadRequest("Invalid profile data");

        //    var profile = await _context.UserProfiles
        //        .FirstOrDefaultAsync(x => x.UserId == userId);

        //    if (profile == null)
        //        return NotFound("Profile not found");

        //    profile.FullName = model.FullName;
        //    profile.PhoneNumber = model.PhoneNumber;
        //    profile.Address = model.Address;
        //    profile.DateOfBirth = model.DateOfBirth;
        //    profile.Gender = model.Gender;
        //    profile.Department = model.Department;
        //    profile.UpdatedAt = DateTime.UtcNow;

        //    await _context.SaveChangesAsync();

        //    return Ok("Profile updated successfully");
        //}

    }
}
