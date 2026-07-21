using System;
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
    public class HolidaysController : ControllerBase
    {
        private readonly LeaveTrackDbContext _context;

        public HolidaysController(LeaveTrackDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HolidayDTO>>> GetAll()
        {
            var holidays = await _context.Holidays
                .OrderBy(h => h.Date)
                .Select(h => new HolidayDTO
                {
                    Id = h.Id,
                    Date = h.Date,
                    Name = h.Name
                }).ToListAsync();

            return Ok(holidays);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HolidayDTO>> GetById(int id)
        {
            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday == null)
                return NotFound();

            var dto = new HolidayDTO
            {
                Id = holiday.Id,
                Date = holiday.Date,
                Name = holiday.Name
            };

            return Ok(dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<HolidayDTO>> Create([FromBody] HolidayDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var holiday = new Holiday
            {
                Date = dto.Date,
                Name = dto.Name
            };

            _context.Holidays.Add(holiday);
            await _context.SaveChangesAsync();

            dto.Id = holiday.Id;
            return CreatedAtAction(nameof(GetById), new { id = holiday.Id }, dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] HolidayDTO dto)
        {
            if (dto == null || id != dto.Id)
                return BadRequest();

            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday == null)
                return NotFound();

            holiday.Date = dto.Date;
            holiday.Name = dto.Name;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday == null)
                return NotFound();

            _context.Holidays.Remove(holiday);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
