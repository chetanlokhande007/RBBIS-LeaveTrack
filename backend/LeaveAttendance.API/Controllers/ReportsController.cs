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
    public class ReportsController : ControllerBase
    {
        private readonly LeaveTrackDbContext _context;

        public ReportsController(LeaveTrackDbContext context)
        {
            _context = context;
        }

        [HttpGet("leave-balance")]
        public async Task<IActionResult> GetLeaveBalances()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;

            IQueryable<Employee> employeeQuery = _context.Employees;

            if (userRole == "Employee" && int.TryParse(employeeIdClaim, out int empId))
            {
                employeeQuery = employeeQuery.Where(e => e.Id == empId);
            }
            else if (userRole == "Manager" && int.TryParse(employeeIdClaim, out int mgrId))
            {
                employeeQuery = employeeQuery.Where(e => e.ManagerId == mgrId || e.Id == mgrId);
            }
            else if (userRole != "Admin")
            {
                return Forbid();
            }

            var employees = await employeeQuery.ToListAsync();
            var leaveTypes = await _context.LeaveTypes.ToListAsync();
            var currentYear = DateTime.Today.Year;

            var reports = new List<EmployeeLeaveReportDTO>();

            foreach (var emp in employees)
            {
                var report = new EmployeeLeaveReportDTO
                {
                    EmployeeId = emp.Id,
                    FullName = emp.FullName
                };

                foreach (var lt in leaveTypes)
                {
                    var approvedRequests = await _context.LeaveRequests
                        .Where(lr => lr.EmployeeId == emp.Id &&
                                     lr.LeaveTypeId == lt.Id &&
                                     lr.Status == LeaveRequestStatus.Approved &&
                                     lr.StartDate.Year == currentYear)
                        .ToListAsync();

                    double usedDays = approvedRequests.Sum(lr => (lr.EndDate.DayNumber - lr.StartDate.DayNumber) + 1.0);

                    report.LeaveSummaries.Add(new LeaveSummaryDTO
                    {
                        LeaveTypeId = lt.Id,
                        LeaveTypeName = lt.Name,
                        TotalQuota = lt.DefaultDaysPerYear,
                        UsedDays = usedDays,
                        RemainingDays = Math.Max(0, lt.DefaultDaysPerYear - usedDays)
                    });
                }

                reports.Add(report);
            }

            return Ok(reports);
        }

        [HttpGet("attendance-summary")]
        public async Task<IActionResult> GetAttendanceSummary([FromQuery] string? startDate, [FromQuery] string? endDate, [FromQuery] int? targetEmployeeId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;

            int empId = 0;
            if (userRole == "Employee")
            {
                if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out empId))
                {
                    return BadRequest("User profile does not contain employee information");
                }
            }
            else if (targetEmployeeId.HasValue)
            {
                // Manager/Admin can query specific employee
                empId = targetEmployeeId.Value;

                if (userRole == "Manager" && int.TryParse(employeeIdClaim, out int mgrId))
                {
                    // Check if employee reports to manager
                    var isReport = await _context.Employees.AnyAsync(e => e.Id == empId && e.ManagerId == mgrId);
                    if (!isReport && empId != mgrId) return Forbid();
                }
            }
            else
            {
                // Fallback to caller's ID if not specified
                if (!int.TryParse(employeeIdClaim, out empId))
                {
                    return BadRequest("Must specify targetEmployeeId");
                }
            }

            DateOnly start = DateOnly.FromDateTime(DateTime.Today.AddDays(-30));
            DateOnly end = DateOnly.FromDateTime(DateTime.Today);

            if (!string.IsNullOrEmpty(startDate) && DateOnly.TryParse(startDate, out var parsedStart))
            {
                start = parsedStart;
            }
            if (!string.IsNullOrEmpty(endDate) && DateOnly.TryParse(endDate, out var parsedEnd))
            {
                end = parsedEnd;
            }

            var records = await _context.Attendances
                .Where(a => a.EmployeeId == empId && a.Date >= start && a.Date <= end)
                .ToListAsync();

            int present = records.Count(a => a.Status == AttendanceStatus.Present);
            int absent = records.Count(a => a.Status == AttendanceStatus.Absent);
            int halfDay = records.Count(a => a.Status == AttendanceStatus.HalfDay);
            int leave = records.Count(a => a.Status == AttendanceStatus.Leave);

            double totalDays = present + absent + halfDay + leave;
            double presentWeight = present + (halfDay * 0.5) + leave;
            double rate = totalDays > 0 ? (presentWeight / totalDays) * 100 : 100;

            var summary = new AttendanceSummaryDTO
            {
                PresentDays = present,
                AbsentDays = absent,
                HalfDays = halfDay,
                LeaveDays = leave,
                AttendanceRate = Math.Round(rate, 1)
            };

            return Ok(summary);
        }
    }
}
