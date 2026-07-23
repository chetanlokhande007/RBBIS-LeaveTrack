using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(CreateUserRequest request);
    }
}
