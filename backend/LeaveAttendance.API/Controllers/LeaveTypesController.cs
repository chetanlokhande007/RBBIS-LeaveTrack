using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeaveAttendance.API.Data;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.DTOs;

namespace LeaveAttendance.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveTypesController : ControllerBase
    {
        private readonly LeaveTrackDbContext _context;

        public LeaveTypesController(LeaveTrackDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveTypeDTO>>> GetAll()
        {
            var types = await _context.LeaveTypes
                .Select(t => new LeaveTypeDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    DefaultDaysPerYear = t.DefaultDaysPerYear
                }).ToListAsync();

            return Ok(types);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveTypeDTO>> GetById(int id)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null)
                return NotFound();

            var dto = new LeaveTypeDTO
            {
                Id = leaveType.Id,
                Name = leaveType.Name,
                DefaultDaysPerYear = leaveType.DefaultDaysPerYear
            };

            return Ok(dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<LeaveTypeDTO>> Create([FromBody] LeaveTypeDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var leaveType = new LeaveType
            {
                Name = dto.Name,
                DefaultDaysPerYear = dto.DefaultDaysPerYear
            };

            _context.LeaveTypes.Add(leaveType);
            await _context.SaveChangesAsync();

            dto.Id = leaveType.Id;
            return CreatedAtAction(nameof(GetById), new { id = leaveType.Id }, dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LeaveTypeDTO dto)
        {
            if (dto == null || id != dto.Id)
                return BadRequest();

            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null)
                return NotFound();

            leaveType.Name = dto.Name;
            leaveType.DefaultDaysPerYear = dto.DefaultDaysPerYear;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null)
                return NotFound();

            _context.LeaveTypes.Remove(leaveType);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
