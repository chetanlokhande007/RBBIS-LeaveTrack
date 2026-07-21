using System;

namespace LeaveAttendance.API.Models
{
    public enum LeaveRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class LeaveRequest
    {
        public int Id { get; set; }
        
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        
        public int LeaveTypeId { get; set; }
        public LeaveType LeaveType { get; set; } = null!;
        
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        
        public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;
        
        public int? ApprovedById { get; set; }
        public Employee? ApprovedBy { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
