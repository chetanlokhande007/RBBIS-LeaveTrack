using System.Collections.Generic;

namespace LeaveAttendance.API.DTOs
{
    public class LeaveSummaryDTO
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public int TotalQuota { get; set; }
        public double UsedDays { get; set; }
        public double RemainingDays { get; set; }
    }

    public class EmployeeLeaveReportDTO
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public List<LeaveSummaryDTO> LeaveSummaries { get; set; } = new();
    }

    public class AttendanceSummaryDTO
    {
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int HalfDays { get; set; }
        public int LeaveDays { get; set; }
        public double AttendanceRate { get; set; }
    }
}
