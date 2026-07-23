using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Services.Interfaces;

namespace LeaveAttendance.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetTodayStatus()
        {
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;
            var result = await _attendanceService.GetTodayStatusAsync(employeeIdClaim);
            return Ok(result);
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn()
        {
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;
            var result = await _attendanceService.CheckInAsync(employeeIdClaim);
            return Ok(result);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut()
        {
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;
            var result = await _attendanceService.CheckOutAsync(employeeIdClaim);
            return Ok(result);
        }

        [HttpGet("my-history")]
        public async Task<ActionResult<IEnumerable<AttendanceDTO>>> GetMyHistory([FromQuery] string? month, [FromQuery] string? year)
        {
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;
            var result = await _attendanceService.GetMyHistoryAsync(employeeIdClaim, month, year);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("team")]
        public async Task<ActionResult<IEnumerable<AttendanceDTO>>> GetTeamAttendance([FromQuery] string? date, [FromQuery] string? department)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;

            var result = await _attendanceService.GetTeamAttendanceAsync(userRole, employeeIdClaim, date, department);
            return Ok(result);
        }
    }
}
