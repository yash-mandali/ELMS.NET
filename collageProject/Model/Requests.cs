using System.ComponentModel.DataAnnotations;

namespace collageProject.Model
{
    public class Requests
    {
        public int LeaveRequestId { get; set; }
        public int UserId { get; set; }       
        public string? RequestType { get; set; }    
        public DateOnly FromDate { get; set; }    
        public DateOnly ToDate { get; set; }
        public double TotalDays { get; set; }
        public string? Session { get; set; }
        public string? Reason { get; set; }
        public string? HandoverTo { get; set; } 
        public string Status { get; set; } = "Pending";
        public DateOnly AppliedOn { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public UserProfile? UserProfile { get; set; }
    }

    public class LeaveRequest
    {
        public int UserId { get; set; }
        public string? RequestType { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public string? Session { get; set; }
        public string? Reason { get; set; }
        public string? HandoverTo { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime AppliedOn { get; set; } = DateTime.Now;

    }
}
