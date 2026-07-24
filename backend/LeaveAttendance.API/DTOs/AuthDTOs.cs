namespace LeaveAttendance.API.DTOs
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public EmployeeCreateUpdateDTO? EmployeeDetails { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        public string Username { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string Username { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class SendOtpRequest
    {
        public string Username { get; set; } = string.Empty;
    }

    public class ResendOtpRequest
    {
        public string Username { get; set; } = string.Empty;
    }

    public class VerifyOtpRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}
