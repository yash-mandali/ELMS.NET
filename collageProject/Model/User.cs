using System.ComponentModel.DataAnnotations;

namespace collageProject.Model
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(70)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "password is required")]
        public string Password { get; set; }

        //[Required(ErrorMessage = "role is required")]
        public string Role { get; set; }
    }
}
