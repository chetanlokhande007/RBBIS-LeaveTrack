using System;
using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.DTOs
{
    public class LeaveTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DefaultDaysPerYear { get; set; }
    }

    public class LeaveRequestDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public LeaveRequestStatus Status { get; set; }
        public int? ApprovedById { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class LeaveRequestCreateDTO
    {
        public int LeaveTypeId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class LeaveRequestDecideDTO
    {
        public LeaveRequestStatus Status { get; set; } // Approved or Rejected
    }
}
