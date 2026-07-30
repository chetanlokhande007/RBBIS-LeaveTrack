using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveAttendance.API.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public int? EmployeeId { get; set; }
        
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }
    }
}
