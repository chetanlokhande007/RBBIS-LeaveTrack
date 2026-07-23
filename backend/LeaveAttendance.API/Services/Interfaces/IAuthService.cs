using LeaveAttendance.API.DTOs;

namespace LeaveAttendance.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<object> RegisterAsync(RegisterRequest request);
        Task<object> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<object> ResetPasswordAsync(ResetPasswordRequest request);
        Task<object> ChangePasswordAsync(ChangePasswordRequest request);
    }
}
