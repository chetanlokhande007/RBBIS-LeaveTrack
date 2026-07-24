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
        private readonly IEmailService _emailService;

        public AuthService(IAuthRepository authRepository, IJwtService jwtService, ILogger<AuthService> logger, IEmailService emailService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
            _logger = logger;
            _emailService = emailService;
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

        private string GenerateOTP()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        private async Task SendOtpEmailAsync(string email, string otp)
        {
            var subject = "Forgot Password OTP";
            var body = $@"Hello,

We received a request to reset your password.

Your One-Time Password (OTP) is:

{otp}

This OTP is valid for 5 minutes.

If you did not request a password reset, please ignore this email.

Regards,
RBIS Team";

            await _emailService.SendEmailAsync(email, subject, body);
        }

        public async Task<object> SendForgotPasswordOtpAsync(SendOtpRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username or Email is required.");

            var user = await _authRepository.GetUserByUsernameOrEmailAsync(request.Username.Trim());
            if (user == null)
            {
                return new { message = "If the account exists, an OTP has been sent to the registered email." };
            }

            bool otpGenerated = false;
            if (string.IsNullOrWhiteSpace(user.PasswordResetToken) || user.PasswordResetExpiry == null || user.PasswordResetExpiry < DateTime.UtcNow)
            {
                user.PasswordResetToken = GenerateOTP();
                user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(5);
                await _authRepository.UpdateUserAsync(user);
                otpGenerated = true;
            }

            try
            {
                if (user.Employee != null && !string.IsNullOrWhiteSpace(user.Employee.Email))
                {
                    await SendOtpEmailAsync(user.Employee.Email, user.PasswordResetToken);
                }
                else
                {
                    // Fallback for users without an email (like admin)
                    _logger.LogWarning("User {Username} requested an OTP but has no registered email. OTP is: {Otp}", user.Username, user.PasswordResetToken);
                    Console.WriteLine($"--- SIMULATED EMAIL TO {user.Username} ---");
                    Console.WriteLine($"Your OTP is: {user.PasswordResetToken}");
                    Console.WriteLine("---------------------------------------");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP to user {Username}", user.Username);
                if (otpGenerated)
                {
                    // Rollback OTP generation if email fails so they can retry immediately without being locked into a failed OTP
                    user.PasswordResetToken = null;
                    user.PasswordResetExpiry = null;
                    await _authRepository.UpdateUserAsync(user);
                }
                throw new InvalidOperationException("Failed to send email. Please check configuration or try again.");
            }

            return new
            {
                message = "If the account exists, an OTP has been sent to the registered email."
            };
        }

        public async Task<object> ResendForgotPasswordOtpAsync(ResendOtpRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username or Email is required.");

            var user = await _authRepository.GetUserByUsernameOrEmailAsync(request.Username.Trim());
            if (user == null)
            {
                return new { message = "If the account exists, an OTP has been sent to the registered email." };
            }

            if (string.IsNullOrWhiteSpace(user.PasswordResetToken) || user.PasswordResetExpiry == null || user.PasswordResetExpiry < DateTime.UtcNow)
            {
                // Expired or missing, generate new
                user.PasswordResetToken = GenerateOTP();
                user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(5);
                await _authRepository.UpdateUserAsync(user);
            }

            if (user.Employee != null && !string.IsNullOrWhiteSpace(user.Employee.Email))
            {
                await SendOtpEmailAsync(user.Employee.Email, user.PasswordResetToken);
            }
            else
            {
                _logger.LogWarning("User {Username} requested an OTP resend but has no registered email. OTP is: {Otp}", user.Username, user.PasswordResetToken);
                Console.WriteLine($"--- SIMULATED EMAIL TO {user.Username} ---");
                Console.WriteLine($"Your OTP is: {user.PasswordResetToken}");
                Console.WriteLine("---------------------------------------");
            }

            return new
            {
                message = "OTP resent successfully."
            };
        }

        public async Task<object> VerifyForgotPasswordOtpAsync(VerifyOtpRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Otp))
                throw new ArgumentException("Username/Email and OTP are required.");

            var user = await _authRepository.GetUserByUsernameOrEmailAsync(request.Username.Trim());
            
            if (user == null || user.PasswordResetToken != request.Otp)
            {
                throw new ArgumentException("Invalid OTP.");
            }

            if (user.PasswordResetExpiry == null || user.PasswordResetExpiry < DateTime.UtcNow)
            {
                throw new ArgumentException("OTP has expired.\nPlease click \"Resend OTP\" to receive a new verification code.");
            }

            return new
            {
                message = "OTP verified successfully.",
                // Return the OTP to be used as token for ResetPassword
                token = request.Otp 
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
