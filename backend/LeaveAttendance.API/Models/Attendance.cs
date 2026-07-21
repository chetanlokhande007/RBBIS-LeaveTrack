using System;

namespace LeaveAttendance.API.Models
{
    public enum AttendanceStatus
    {
        Present,
        Absent,
        HalfDay,
        Leave
    }

    public class Attendance
    {
        public int Id { get; set; }
        
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        
        public DateOnly Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent;
    }
}
