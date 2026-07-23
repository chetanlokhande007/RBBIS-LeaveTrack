using System.Collections.Generic;
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
    public class LeaveTypesController : ControllerBase
    {
        private readonly ILeaveTypeService _leaveTypeService;

        public LeaveTypesController(ILeaveTypeService leaveTypeService)
        {
            _leaveTypeService = leaveTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveTypeDTO>>> GetAll()
        {
            var types = await _leaveTypeService.GetAllLeaveTypesAsync();
            return Ok(types);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveTypeDTO>> GetById(int id)
        {
            var dto = await _leaveTypeService.GetLeaveTypeByIdAsync(id);
            return Ok(dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<LeaveTypeDTO>> Create([FromBody] LeaveTypeDTO dto)
        {
            var created = await _leaveTypeService.CreateLeaveTypeAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LeaveTypeDTO dto)
        {
            await _leaveTypeService.UpdateLeaveTypeAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _leaveTypeService.DeleteLeaveTypeAsync(id);
            return NoContent();
        }
    }
}
