namespace collageProject.Model
{
    public class EmailOtp
    {
        public int Id { get; set; }

        public string? Email { get; set; }

        public string? OtpCode { get; set; }

        public DateTime? ExpiryTime { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UsedAt { get; set; }
    }

    public class EmailRequest
    {
        public string? Email { get; set; }
    }

    public class EmaillVerify
    {
        public string? Email { get; set; }

        public string? OtpCode { get; set; }
    }
}
