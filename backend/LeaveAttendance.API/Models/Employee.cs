using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LeaveAttendance.API.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        
        public int? ManagerId { get; set; }
        
        [JsonIgnore]
        public Employee? Manager { get; set; }
        
        [JsonIgnore]
        public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();
        
        public DateOnly DateOfJoining { get; set; }
    }
}
