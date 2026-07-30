using LeaveAttendance.API.DTOs;

namespace LeaveAttendance.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto request);
        Task<object> RegisterAsync(RegisterDto request);
        Task<object> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<object> ResetPasswordAsync(ResetPasswordRequest request);
        Task<object> ChangePasswordAsync(ChangePasswordRequest request);
        Task<object> SendForgotPasswordOtpAsync(SendOtpRequest request);
        Task<object> ResendForgotPasswordOtpAsync(ResendOtpRequest request);
        Task<object> VerifyForgotPasswordOtpAsync(VerifyOtpRequest request);
    }
}
