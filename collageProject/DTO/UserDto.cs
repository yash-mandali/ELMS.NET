using collageProject.Model;

namespace collageProject.DTO
{
    public class UserDto
    {
        //public int Id { get; set; }

        //[Required(ErrorMessage = "Name is required")]
        //[MaxLength(70)]
        //public string UserName { get; set; }

        //[Required(ErrorMessage = "email is required")]
        //[EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "password is required")]
        //public string Password { get; set; }

        //[Required(ErrorMessage = "role is required")]
        public string Role { get; set; }

    }

    public class AddUserDto
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "Name is required")]
        //[MaxLength(70)]
        public string? UserName { get; set; }

        //[Required(ErrorMessage = "email is required")]
        //[EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        //[Required(ErrorMessage = "password is required")]
        public string? Password { get; set; }

        //[Required(ErrorMessage = "role is required")]
        public string? Role { get; set; }

        //public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsUserUpdated { get; set; }
        public DateTime? UserUpdatedAt { get; set; }
    }

    public class CreateProfileRequest
    {
        public UserProfile UserProfile { get; set; }
        public UserDto User { get; set; }
    }

}
