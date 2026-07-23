using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role?> GetRoleByIdAsync(int id);
    }
}
