using System;
using System.Linq;
using System.Security.Claims;
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
    public class EmployeesController : ControllerBase
    {
        private readonly LeaveTrackDbContext _context;

        public EmployeesController(LeaveTrackDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;

            if (userRole == "Admin" || userRole == "HR")
            {
                var employees = await _context.Employees
                    .Include(e => e.Manager)
                    .Select(e => new EmployeeDTO
                    {
                        Id = e.Id,
                        FullName = e.FullName,
                        Email = e.Email,
                        Department = e.Department,
                        Designation = e.Designation,
                        ManagerId = e.ManagerId,
                        ManagerName = e.Manager != null ? e.Manager.FullName : null,
                        DateOfJoining = e.DateOfJoining
                    }).ToListAsync();
                return Ok(employees);
            }
            else if (userRole == "Manager" && int.TryParse(employeeIdClaim, out int managerId))
            {
                var reports = await _context.Employees
                    .Where(e => e.ManagerId == managerId || e.Id == managerId)
                    .Include(e => e.Manager)
                    .Select(e => new EmployeeDTO
                    {
                        Id = e.Id,
                        FullName = e.FullName,
                        Email = e.Email,
                        Department = e.Department,
                        Designation = e.Designation,
                        ManagerId = e.ManagerId,
                        ManagerName = e.Manager != null ? e.Manager.FullName : null,
                        DateOfJoining = e.DateOfJoining
                    }).ToListAsync();
                return Ok(reports);
            }
            else if (userRole == "Employee" && int.TryParse(employeeIdClaim, out int empId))
            {
                var employee = await _context.Employees
                    .Where(e => e.Id == empId)
                    .Include(e => e.Manager)
                    .Select(e => new EmployeeDTO
                    {
                        Id = e.Id,
                        FullName = e.FullName,
                        Email = e.Email,
                        Department = e.Department,
                        Designation = e.Designation,
                        ManagerId = e.ManagerId,
                        ManagerName = e.Manager != null ? e.Manager.FullName : null,
                        DateOfJoining = e.DateOfJoining
                    }).FirstOrDefaultAsync();

                if (employee == null) return NotFound();
                return Ok(new[] { employee });
            }

            return Forbid();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;

            if (userRole == "Employee" && employeeIdClaim != id.ToString())
            {
                return Forbid();
            }

            if (userRole == "Manager" && employeeIdClaim != id.ToString())
            {
                // Check if employee reports to this manager
                int managerId = int.Parse(employeeIdClaim!);
                var isReport = await _context.Employees.AnyAsync(e => e.Id == id && e.ManagerId == managerId);
                if (!isReport) return Forbid();
            }

            var employee = await _context.Employees
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return NotFound();

            var dto = new EmployeeDTO
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Department = employee.Department,
                Designation = employee.Designation,
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager?.FullName,
                DateOfJoining = employee.DateOfJoining
            };

            return Ok(dto);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeCreateUpdateDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var employee = new Employee
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Department = dto.Department,
                Designation = dto.Designation,
                ManagerId = dto.ManagerId,
                DateOfJoining = dto.DateOfJoining
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, new EmployeeDTO
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Department = employee.Department,
                Designation = employee.Designation,
                ManagerId = employee.ManagerId,
                DateOfJoining = employee.DateOfJoining
            });
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeCreateUpdateDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            employee.FullName = dto.FullName;
            employee.Email = dto.Email;
            employee.Department = dto.Department;
            employee.Designation = dto.Designation;
            employee.ManagerId = dto.ManagerId;
            employee.DateOfJoining = dto.DateOfJoining;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            // We can check if employee has dependent records, or cascade deletes
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
