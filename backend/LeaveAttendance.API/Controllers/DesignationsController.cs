using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeaveAttendance.API.Data;
using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DesignationsController : ControllerBase
    {
        private readonly LeaveTrackDbContext _context;

        public DesignationsController(LeaveTrackDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Designation>>> GetAll()
        {
            return await _context.Designations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Designation>> GetById(int id)
        {
            var desig = await _context.Designations.FindAsync(id);
            if (desig == null) return NotFound();
            return Ok(desig);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        public async Task<ActionResult<Designation>> Create([FromBody] Designation desig)
        {
            desig.CreatedAt = System.DateTime.UtcNow;
            _context.Designations.Add(desig);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = desig.Id }, desig);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Designation desig)
        {
            if (id != desig.Id) return BadRequest();
            _context.Entry(desig).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var desig = await _context.Designations.FindAsync(id);
            if (desig == null) return NotFound();
            _context.Designations.Remove(desig);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
