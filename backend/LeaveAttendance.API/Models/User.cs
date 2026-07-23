using System;
using System.Text.Json.Serialization;

namespace LeaveAttendance.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;
        
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
        
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpiry { get; set; }
    }
}
