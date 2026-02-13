using collageProject;
using collageProject.DataContext;
using collageProject.Model;
using collageProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace AngularLoginApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly GetJwtToken _getToken;
        private readonly IConfiguration _configuration;
        //private readonly GeminiService _gemini;
        public readonly EmailOtpService _email;

        public UserController
            (AppDbContext context,
            IConfiguration configuration,
            GetJwtToken getToken,
            //GeminiService gemini,
            EmailOtpService email
            )
        {
            _context = context;
            _configuration = configuration;
            _getToken = getToken;
            //_gemini = gemini;
            _email = email;
        }

       

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet]
        [Route("GetUserById")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var Data = await _context.Users.FindAsync(id);
                if (Data == null)
                {
                    return Ok(new { Message = "Data Not Found", Status = 401, data = new { } });
                }
                else
                {
                    return Ok(new { Status = 200, Data });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message.ToString(), Status = 401, data = new { } });
            }
        }

        //[Authorize]
        //[HttpGet("getUserprofile")]
        //public async Task<IActionResult> GetUserProfile()
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if(userIdClaim == null)
        //    {
        //        return Unauthorized("Invalid Token");
        //    }
        //    int userId = int.Parse(userIdClaim.Value);

        //    var user = await _context.Users.FindAsync(userId);

        //    if (user == null)
        //    {
        //        return NotFound("User not found");
        //    }

        //    var profile = new User
        //    {
        //        Id = user.Id,
        //        UserName = user.UserName,
        //        Email = user.Email,
        //        Role = user.Role
        //    };

        //    return Ok(new {message="UserProfile" ,profile});
        //}

        [HttpPost]
        [Route("AddUser")]
        public async Task<IActionResult> Signup(User users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = _context.Users.Where(u => (u.Email == users.Email)).FirstOrDefault();
                if (user != null)
                {
                    return Conflict(new { message = "User already exists" });
                }

                users.Password = BCrypt.Net.BCrypt.HashPassword(users.Password);

                _context.Users.Add(users);
                await _context.SaveChangesAsync();
                return Ok(new { message = "User Added successfull" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }

        }

        [HttpPost]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser(User user, int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new {message = "Invalid model"});
                }
                var userid = await _context.Users.FindAsync(id);

                if(userid == null)
                {
                    return Ok(new { Message = "UserData Not Found", Status = 401, data = new { } });
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                userid.UserName = user.UserName;
                userid.Email = user.Email;
                userid.Password = user.Password;
                userid.IsUserUpdated = true;
                userid.UserUpdatedAt = DateTime.Today;
                

                await _context.SaveChangesAsync();
                return Ok(new { status = 200, message = "User updated succesfully" });
                
            } 
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message.ToString(), Status = 401, data = new { } });
            }
           
        }

        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var Data = await _context.Users.FindAsync(id);
                if (Data == null)
                {
                    return Ok(new { Message = "Data Not Found", Status = 401 });
                }
                else
                {
                    _context.Users.Remove(Data);
                    await _context.SaveChangesAsync();
                    return Ok(new { Status = 200,message = "User Removed"});
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message.ToString(), Status = 401, data = new { } });
            }
        }

        [HttpPost]
        [Route("login")]    
        public async Task<IActionResult> Login(Login users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == users.Email);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email" });
                }

                if (user.Role != users.Role)
                {
                    return Unauthorized(new { message = "Authentication failed" });
                }

                //----------- or ----------------

                //if (!string.Equals(user.Role, users.Role, StringComparison.OrdinalIgnoreCase))
                //{
                //    return Unauthorized(new { message = "Authentication failed" });
                //}

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(users.Password, user.Password);

                if (!isPasswordValid)
                {
                    return Unauthorized(new { message = "Invalid  password" });

                }
                var Token = _getToken.GenerateJwtToken(users);

                //return Ok(new { message = "Login successfull" });
                return Ok(new { message = "Login successfull", token = Token, role = user.Role });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ForgetPassword(string email, string newPassword, string conformPassword)
        {
            //check model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                //check email 
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email" });
                }

                if (newPassword != conformPassword)
                {
                    return BadRequest(new { message = "Password does not match" });
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Password updated" });
            }
            catch
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost]
        [Route("EmailOtp")]
        public async Task<IActionResult> sendOtpEmail(string email)
        {
            try 
            {
                if (email != "")
                {
                    var checkUserEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (checkUserEmail == null)
                    {
                        return Unauthorized(new { message = "Invalid email" });
                    }
                    else
                    {
                        string username = checkUserEmail.UserName;
                        var otp = _email.GenerateOTP();

                        EmailOtp emailOtp = new EmailOtp
                        {
                            Email = email,
                            OtpCode = otp,
                            ExpiryTime = DateTime.UtcNow.AddMinutes(5)
                        };

                        _context.EmailOtp.Add(emailOtp);
                        await _context.SaveChangesAsync();


                        bool isEmailsent = await _email.sendOtpEmail(email, otp, username);
                        if (isEmailsent)
                        {
                            return Ok(new { Status = true, otp, message = "OTP sent successfully" });
                        }
                        else
                        {
                            return StatusCode(500, new { Status = false, message = "Failed to send OTP email" });
                        }
                    }
                }
                else
                {
                    return Ok(new { message = "please enter email id" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Error = ex.Message,
                    Inner = ex.InnerException?.Message
                });
            }

        }

        [HttpPost]
        [Route("verifyOtp")]
        public async Task<IActionResult> verifyOtp(string email,string otp)
        {
            try
            {
                if (email != "" && otp != "") {

                    var emailOtpCheck = await _context.EmailOtp.Where(e => e.Email == email && e.OtpCode == otp && !e.IsUsed)
                        .OrderByDescending(e => e.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (emailOtpCheck == null)
                    {
                        return Unauthorized(new { Status = false, Message = "Invalid OTP or already used" });
                    }
                    // Check if OTP is expired
                    if (DateTime.UtcNow > emailOtpCheck.ExpiryTime)
                    {
                        return BadRequest(new { Status = false, Message = "OTP has expired" });
                    }

                    emailOtpCheck.IsUsed = true;
                    _context.EmailOtp.Update(emailOtpCheck);
                    await _context.SaveChangesAsync();

                    return Ok(new { Status = true, Message = "OTP verified successfully" });
                }
                else
                {
                    return BadRequest(new { Status = false, Message = "Email and OTP are required" });

                }
            } 
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }



        //[HttpPost]
        //[Route("ask")]
        //public async Task<IActionResult> Ask([FromBody] AIRequest request)
        //{
        //    var result = await _gemini.AskGemini(request.prompt);
        //    return Ok(result);
        //}

        //[HttpPost]
        //[Route("SendEmail")]
        //public async Task<IActionResult> sendemail(ContactEmailservice emailModel)
        //{
        //    await _email.sendEmail(emailModel.Email, emailModel.Message);
        //    return Ok(new { message = "Email sent..." });
        //}

    }
}

//public class AIRequest
//{
//    public string prompt { get; set; }
//}
