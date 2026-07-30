using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LeaveAttendance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // =====================================================
        // LOGIN
        // POST: api/Auth/login
        // =====================================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }

        // =====================================================
        // REGISTER
        // POST: api/Auth/register
        // =====================================================
        [AllowAnonymous] // allowing for initial testing based on user flow
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }

        // =====================================================
        // FORGOT PASSWORD
        // POST: api/Auth/forgot-password
        // =====================================================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var response = await _authService.ForgotPasswordAsync(request);
            return Ok(response);
        }

        // =====================================================
        // SEND FORGOT PASSWORD OTP
        // POST: api/Auth/send-forgot-password-otp
        // =====================================================
        [HttpPost("send-forgot-password-otp")]
        public async Task<IActionResult> SendForgotPasswordOtp([FromBody] SendOtpRequest request)
        {
            var response = await _authService.SendForgotPasswordOtpAsync(request);
            return Ok(response);
        }

        // =====================================================
        // RESEND FORGOT PASSWORD OTP
        // POST: api/Auth/resend-forgot-password-otp
        // =====================================================
        [HttpPost("resend-forgot-password-otp")]
        public async Task<IActionResult> ResendForgotPasswordOtp([FromBody] ResendOtpRequest request)
        {
            var response = await _authService.ResendForgotPasswordOtpAsync(request);
            return Ok(response);
        }

        // =====================================================
        // VERIFY FORGOT PASSWORD OTP
        // POST: api/Auth/verify-forgot-password-otp
        // =====================================================
        [HttpPost("verify-forgot-password-otp")]
        public async Task<IActionResult> VerifyForgotPasswordOtp([FromBody] VerifyOtpRequest request)
        {
            var response = await _authService.VerifyForgotPasswordOtpAsync(request);
            return Ok(response);
        }

        // =====================================================
        // RESET PASSWORD
        // POST: api/Auth/reset-password
        // =====================================================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var response = await _authService.ResetPasswordAsync(request);
            return Ok(response);
        }

        // =====================================================
        // CHANGE PASSWORD
        // POST: api/Auth/change-password
        // =====================================================
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var response = await _authService.ChangePasswordAsync(request);
            return Ok(response);
        }
    }
}