using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Repositories.Interfaces;
using LeaveAttendance.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace LeaveAttendance.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, IJwtService jwtService, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Invalid client request or missing credentials.");
            }

            var user = await _authRepository.GetUserByUsernameAsync(request.Username.Trim());

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Role = user.Role?.Name ?? "Employee",
                EmployeeId = user.EmployeeId,
                FullName = user.Employee?.FullName ?? "Administrator"
            };
        }

        public async Task<object> RegisterAsync(RegisterRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Username and password are required.");
            }

            string username = request.Username.Trim();

            if (await _authRepository.UsernameExistsAsync(username))
            {
                throw new InvalidOperationException("Username already exists.");
            }

            var role = await _authRepository.GetRoleByIdAsync(request.RoleId);
            if (role == null)
            {
                throw new ArgumentException("Invalid role ID.");
            }

            Employee? employee = null;
            if (request.EmployeeDetails != null)
            {
                employee = new Employee
                {
                    FullName = request.EmployeeDetails.FullName,
                    Email = request.EmployeeDetails.Email,
                    Department = request.EmployeeDetails.Department,
                    Designation = request.EmployeeDetails.Designation,
                    ManagerId = request.EmployeeDetails.ManagerId,
                    DateOfJoining = request.EmployeeDetails.DateOfJoining
                };
            }

            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = request.RoleId,
            };

            await _authRepository.CreateUserWithEmployeeAsync(user, employee);

            _logger.LogInformation("User {Username} registered successfully.", user.Username);

            return new
            {
                message = "Registration successful",
                username = user.Username,
                employeeId = user.EmployeeId
            };
        }

        public async Task<object> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username))
            {
                throw new ArgumentException("Username is required.");
            }

            var user = await _authRepository.GetUserByUsernameAsync(request.Username.Trim());

            if (user == null)
            {
                // Do not reveal whether username exists
                return new { message = "If the username exists, a password reset link has been generated." };
            }

            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.PasswordResetExpiry = DateTime.UtcNow.AddHours(1);

            await _authRepository.UpdateUserAsync(user);

            return new
            {
                message = "If the username exists, a password reset link has been generated.",
                simulation_token = token // For simulation purposes as per original code
            };
        }

        public async Task<object> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new ArgumentException("Invalid request. Token and new password are required.");
            }

            var user = await _authRepository.GetUserByResetTokenAsync(request.Token);

            if (user == null || user.PasswordResetExpiry == null || user.PasswordResetExpiry < DateTime.UtcNow)
            {
                throw new ArgumentException("Invalid or expired reset token.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetExpiry = null;

            await _authRepository.UpdateUserAsync(user);

            return new { message = "Password has been successfully reset." };
        }

        public async Task<object> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new ArgumentException("Invalid request. All fields are required.");
            }

            var user = await _authRepository.GetUserByUsernameAsync(request.Username.Trim());

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or old password.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            await _authRepository.UpdateUserAsync(user);

            return new { message = "Password changed successfully." };
        }
    }
}
