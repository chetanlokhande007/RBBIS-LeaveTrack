using System;

namespace LeaveAttendance.API.DTOs
{
    public class HolidayDTO
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
