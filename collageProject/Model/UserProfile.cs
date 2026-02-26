using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace collageProject.Model
{
    public class UserProfile
    {
        public int ProfileId { get; set; }

        public int UserId { get; set; }
        public string? Employee_Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Emergency_Phone { get; set; }
        public string? Address { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? Work_Email { get; set; }
        public string? Employment_Type { get; set; }
        public string? Work_Location { get; set; }
        public string? Work_Time { get; set; }
        public string? Manager { get; set; }
        public string? Experience { get; set; }
        public string? Role { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? Joining_date { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
        public User? User { get; set; }
  
    }

    public class UpdateUserProfile {
        public string FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Emergency_Phone { get; set; }
        public string? Work_Email { get; set; }
        public string? Address { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? Employment_Type { get; set; }
        public string? Work_Location { get; set; }
        public string? Work_Time { get; set; }
        public string? Manager { get; set; }
        public string? Experience { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}





