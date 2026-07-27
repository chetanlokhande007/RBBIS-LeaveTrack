using System;

namespace LeaveAttendance.API.Models
{
    public class LeaveApproval
    {
        public int Id { get; set; }
        
        public int LeaveRequestId { get; set; }
        public LeaveRequest LeaveRequest { get; set; } = null!;
        
        public int ApproverId { get; set; }
        public Employee Approver { get; set; } = null!;
        
        public LeaveRequestStatus Action { get; set; } // e.g. Approved, Rejected
        
        public string? Remarks { get; set; }
        
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    }
}
