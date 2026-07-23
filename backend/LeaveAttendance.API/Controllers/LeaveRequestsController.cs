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
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestsController(ILeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestDTO>>> GetAll([FromQuery] string? status)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;

            var requests = await _leaveRequestService.GetAllLeaveRequestsAsync(userRole, employeeIdClaim, status);
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequestDTO>> GetById(int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;

            var request = await _leaveRequestService.GetLeaveRequestByIdAsync(id, userRole, employeeIdClaim);
            return Ok(request);
        }

        [HttpPost]
        public async Task<IActionResult> Apply([FromBody] LeaveRequestCreateDTO dto)
        {
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;
            var result = await _leaveRequestService.ApplyForLeaveAsync(dto, employeeIdClaim);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}/decide")]
        public async Task<IActionResult> Decide(int id, [FromBody] LeaveRequestDecideDTO dto)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var deciderEmpIdClaim = User.FindFirst("employeeId")?.Value;

            var result = await _leaveRequestService.DecideLeaveRequestAsync(id, dto, userRole, deciderEmpIdClaim);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;
            var result = await _leaveRequestService.CancelLeaveRequestAsync(id, employeeIdClaim);
            return Ok(result);
        }
    }
}
