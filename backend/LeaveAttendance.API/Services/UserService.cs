using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Repositories.Interfaces;
using LeaveAttendance.API.Services.Interfaces;

namespace LeaveAttendance.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> CreateUserAsync(CreateUserRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException("Request cannot be null.");
            }

            var employee = await _userRepository.GetEmployeeByIdAsync(request.EmployeeId);

            if (employee == null)
            {
                throw new ArgumentException("Employee not found.");
            }

            var user = new User
            {
                Username = employee.Email, // Ensure Username maps to Employee Email
                EmployeeId = employee.Id,
                RoleId = request.RoleId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            return await _userRepository.CreateUserAsync(user);
        }
    }
}
