using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeaveAttendance.API.Data;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace LeaveAttendance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LeaveTrackDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(
            LeaveTrackDbContext context,
            IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // =====================================================
        // LOGIN
        // POST: api/Auth/login
        // =====================================================
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            // Validate request
            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Invalid client request"
                });
            }

            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest(new
                {
                    message = "Username is required"
                });
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    message = "Password is required"
                });
            }

            string username = request.Username.Trim();

            // Find user
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u =>
                    u.Username.ToLower() == username.ToLower());

            // User not found
            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password"
                });
            }

            // Verify BCrypt password
            bool passwordValid = BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash
            );

            if (!passwordValid)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password"
                });
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Role = user.Role?.Name ?? "Employee",
                EmployeeId = user.EmployeeId,
                FullName = user.Employee?.FullName ?? "Administrator"
            };

            return Ok(response);
        }


        // =====================================================
        // REGISTER
        // POST: api/Auth/register
        // =====================================================
        [Authorize(Roles = "Admin,HR")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            // Validate request
            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Invalid client request"
                });
            }

            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest(new
                {
                    message = "Username is required"
                });
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    message = "Password is required"
                });
            }

            string username = request.Username.Trim();

            // =================================================
            // CHECK DUPLICATE USERNAME
            // =================================================
            bool usernameExists = await _context.Users
                .AnyAsync(u =>
                    u.Username.ToLower() == username.ToLower());

            if (usernameExists)
            {
                return Conflict(new
                {
                    message = "Username already exists"
                });
            }

            // =================================================
            // CHECK ROLE
            // =================================================
            var role = await _context.Roles
                .FindAsync(request.RoleId);

            if (role == null)
            {
                return BadRequest(new
                {
                    message = "Invalid role ID"
                });
            }

            Employee? employee = null;

            // =================================================
            // CREATE EMPLOYEE
            // =================================================
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

                _context.Employees.Add(employee);

                // Save employee first to generate Employee ID
                await _context.SaveChangesAsync();
            }

            // =================================================
            // HASH PASSWORD
            // =================================================
            string hashedPassword =
                BCrypt.Net.BCrypt.HashPassword(request.Password);

            // =================================================
            // CREATE USER
            // =================================================
            var user = new User
            {
                // Keep the username entered by the user
                Username = username,

                // Store ONLY BCrypt hash
                PasswordHash = hashedPassword,

                RoleId = request.RoleId,

                EmployeeId = employee?.Id
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful",
                username = user.Username,
                employeeId = user.EmployeeId
            });
        }


        // =====================================================
        // FORGOT PASSWORD
        // POST: api/Auth/forgot-password
        // =====================================================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest(new
                {
                    message = "Username is required"
                });
            }

            string username = request.Username.Trim();

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username.ToLower() ==
                    username.ToLower());

            // Do not reveal whether username exists
            if (user == null)
            {
                return Ok(new
                {
                    message =
                        "If the username exists, a password reset link has been generated."
                });
            }

            // Generate reset token
            var token = Guid.NewGuid().ToString();

            user.PasswordResetToken = token;

            user.PasswordResetExpiry =
                DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message =
                    "If the username exists, a password reset link has been generated.",

                // Simulation only
                simulation_token = token
            });
        }


        // =====================================================
        // RESET PASSWORD
        // POST: api/Auth/reset-password
        // =====================================================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Invalid client request"
                });
            }

            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return BadRequest(new
                {
                    message = "Reset token is required"
                });
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(new
                {
                    message = "New password is required"
                });
            }

            // Find user by reset token
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.PasswordResetToken ==
                    request.Token);

            // Validate token
            if (user == null ||
                user.PasswordResetExpiry == null ||
                user.PasswordResetExpiry < DateTime.UtcNow)
            {
                return BadRequest(new
                {
                    message = "Invalid or expired reset token."
                });
            }

            // Hash new password
            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    request.NewPassword
                );

            // Clear reset token
            user.PasswordResetToken = null;
            user.PasswordResetExpiry = null;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Password has been successfully reset."
            });
        }
    }
}