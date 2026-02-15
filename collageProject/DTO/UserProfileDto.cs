namespace collageProject.DTO
{
    public class UserProfileDto
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Department { get; set; }
        public string? Role { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
