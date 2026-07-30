using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user, System.Collections.Generic.IList<string> roles);
    }
}
