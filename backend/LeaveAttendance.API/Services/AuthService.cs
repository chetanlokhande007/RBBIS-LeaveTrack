using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Data;
using LeaveAttendance.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace LeaveAttendance.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly LeaveTrackDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            LeaveTrackDbContext context,
            IJwtService jwtService,
            ILogger<AuthService> logger,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Invalid client request or missing credentials.");
            }

            var user = await _userManager.FindByEmailAsync(request.Email.Trim());

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Employee";
            
            var token = _jwtService.GenerateToken(user, role);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                Role = role
            };
        }

        public async Task<object> RegisterAsync(RegisterDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Email and password are required.");
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email.Trim());
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            // Create ApplicationUser
            var user = new ApplicationUser
            {
                UserName = request.Email.Trim(),
                Email = request.Email.Trim(),
                PhoneNumber = request.Phone
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Registration failed: {errors}");
            }

            // Assign Role
            var roleName = string.IsNullOrWhiteSpace(request.Role) ? "Employee" : request.Role;
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
            await _userManager.AddToRoleAsync(user, roleName);

            // Create Employee Profile
            var employee = new Employee
            {
                UserId = user.Id,
                FullName = request.Name,
                Email = request.Email.Trim(),
                Department = request.Department,
                DateOfJoining = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {Email} registered successfully.", user.Email);

            return new
            {
                message = "Registration successful"
            };
        }

        public async Task<object> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            // Placeholder for now
            throw new NotImplementedException();
        }

        public async Task<object> SendForgotPasswordOtpAsync(SendOtpRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<object> ResendForgotPasswordOtpAsync(ResendOtpRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<object> VerifyForgotPasswordOtpAsync(VerifyOtpRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<object> ResetPasswordAsync(ResetPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<object> ChangePasswordAsync(ChangePasswordRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
