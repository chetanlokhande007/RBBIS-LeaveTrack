using System;

namespace LeaveAttendance.API.Models
{
    public class Holiday
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
