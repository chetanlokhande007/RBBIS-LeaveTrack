using System;
using System.Collections.Generic;
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
    public class AttendanceController : ControllerBase
    {
        private readonly LeaveTrackDbContext _context;

        public AttendanceController(LeaveTrackDbContext context)
        {
            _context = context;
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetTodayStatus()
        {
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int empId))
            {
                return BadRequest("User profile does not contain employee information");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == empId && a.Date == today);

            if (attendance == null)
            {
                return Ok(new { checkedIn = false, checkInTime = (DateTime?)null, checkOutTime = (DateTime?)null });
            }

            return Ok(new
            {
                checkedIn = attendance.CheckInTime != null && attendance.CheckOutTime == null,
                checkInTime = attendance.CheckInTime,
                checkOutTime = attendance.CheckOutTime,
                status = attendance.Status.ToString()
            });
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn()
        {
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int empId))
            {
                return BadRequest("User profile does not contain employee information");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == empId && a.Date == today);

            if (attendance != null)
            {
                if (attendance.CheckInTime != null)
                {
                    return BadRequest("Already checked in today");
                }
                attendance.CheckInTime = DateTime.UtcNow;
                attendance.Status = AttendanceStatus.Present;
            }
            else
            {
                attendance = new Attendance
                {
                    EmployeeId = empId,
                    Date = today,
                    CheckInTime = DateTime.UtcNow,
                    CheckOutTime = null,
                    Status = AttendanceStatus.Present
                };
                _context.Attendances.Add(attendance);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Checked in successfully", checkInTime = attendance.CheckInTime });
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut()
        {
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int empId))
            {
                return BadRequest("User profile does not contain employee information");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == empId && a.Date == today);

            if (attendance == null || attendance.CheckInTime == null)
            {
                return BadRequest("You must check in first before checking out");
            }

            if (attendance.CheckOutTime != null)
            {
                return BadRequest("Already checked out today");
            }

            attendance.CheckOutTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Checked out successfully", checkOutTime = attendance.CheckOutTime });
        }

        [HttpGet("my-history")]
        public async Task<ActionResult<IEnumerable<AttendanceDTO>>> GetMyHistory([FromQuery] string? month, [FromQuery] string? year)
        {
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int empId))
            {
                return BadRequest("User profile does not contain employee information");
            }

            IQueryable<Attendance> query = _context.Attendances
                .Where(a => a.EmployeeId == empId);

            if (int.TryParse(month, out int m) && int.TryParse(year, out int y))
            {
                var startDate = new DateOnly(y, m, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                query = query.Where(a => a.Date >= startDate && a.Date <= endDate);
            }

            var history = await query
                .OrderByDescending(a => a.Date)
                .Select(a => new AttendanceDTO
                {
                    Id = a.Id,
                    EmployeeId = a.EmployeeId,
                    EmployeeName = a.Employee.FullName,
                    Date = a.Date,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    Status = a.Status
                }).ToListAsync();

            return Ok(history);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("team")]
        public async Task<ActionResult<IEnumerable<AttendanceDTO>>> GetTeamAttendance([FromQuery] string? date, [FromQuery] string? department)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;

            IQueryable<Attendance> query = _context.Attendances
                .Include(a => a.Employee);

            // Filter by date range or specific date (default: today)
            var targetDate = DateOnly.FromDateTime(DateTime.Today);
            if (!string.IsNullOrEmpty(date) && DateOnly.TryParse(date, out var parsedDate))
            {
                targetDate = parsedDate;
            }
            query = query.Where(a => a.Date == targetDate);

            // Role filtering
            if (userRole == "Manager" && int.TryParse(employeeIdClaim, out int mgrId))
            {
                query = query.Where(a => a.Employee.ManagerId == mgrId);
            }
            else if (userRole != "Admin")
            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(a => a.Employee.Department.ToLower() == department.ToLower());
            }

            var records = await query
                .Select(a => new AttendanceDTO
                {
                    Id = a.Id,
                    EmployeeId = a.EmployeeId,
                    EmployeeName = a.Employee.FullName,
                    Date = a.Date,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    Status = a.Status
                }).ToListAsync();

            return Ok(records);
        }
    }
}
