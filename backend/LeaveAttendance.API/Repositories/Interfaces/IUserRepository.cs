using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<Employee?> GetEmployeeByIdAsync(int employeeId);
    }
}
