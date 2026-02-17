
using System.ComponentModel.DataAnnotations;

namespace collageProject.Model
{
    public class User
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

        public DateTime CreatedAt { get; set; }

        // Navigation Property (1-to-1)
        public UserProfile UserProfile { get; set; }

    }

    public class AddUser
    {
        public string? UserName { get; set; }      
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public bool IsUserUpdated { get; set; }
        public DateTime? UserUpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateUser
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
