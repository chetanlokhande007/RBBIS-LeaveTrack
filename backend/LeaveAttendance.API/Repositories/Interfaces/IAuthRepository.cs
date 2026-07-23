using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByResetTokenAsync(string token);
        Task<bool> UsernameExistsAsync(string username);
        Task<Role?> GetRoleByIdAsync(int roleId);
        Task CreateUserWithEmployeeAsync(User user, Employee? employee);
        Task UpdateUserAsync(User user);
    }
}
