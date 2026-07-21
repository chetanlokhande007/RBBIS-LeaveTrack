using System;

namespace LeaveAttendance.API.DTOs
{
    public class EmployeeDetailsRequest
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;

        public int? ManagerId { get; set; }

        public DateTime DateOfJoining { get; set; }
    }
}