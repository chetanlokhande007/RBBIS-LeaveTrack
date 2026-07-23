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
    public class HolidaysController : ControllerBase
    {
        private readonly IHolidayService _holidayService;

        public HolidaysController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HolidayDTO>>> GetAll()
        {
            var holidays = await _holidayService.GetAllHolidaysAsync();
            return Ok(holidays);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HolidayDTO>> GetById(int id)
        {
            var dto = await _holidayService.GetHolidayByIdAsync(id);
            return Ok(dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<HolidayDTO>> Create([FromBody] HolidayDTO dto)
        {
            var created = await _holidayService.CreateHolidayAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] HolidayDTO dto)
        {
            await _holidayService.UpdateHolidayAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _holidayService.DeleteHolidayAsync(id);
            return NoContent();
        }
    }
}
