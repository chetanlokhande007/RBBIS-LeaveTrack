using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user, string role);
    }
}
