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
    public class LeaveRequestsController : ControllerBase
    {
        private readonly LeaveTrackDbContext _context;

        public LeaveRequestsController(LeaveTrackDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestDTO>>> GetAll([FromQuery] string? status)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;

            IQueryable<LeaveRequest> query = _context.LeaveRequests
                .Include(lr => lr.Employee)
                .Include(lr => lr.LeaveType)
                .Include(lr => lr.ApprovedBy);

            if (userRole == "Employee" && int.TryParse(employeeIdClaim, out int empId))
            {
                query = query.Where(lr => lr.EmployeeId == empId);
            }
            else if (userRole == "Manager" && int.TryParse(employeeIdClaim, out int mgrId))
            {
                // Manager sees requests of direct reports and themselves
                query = query.Where(lr => lr.Employee.ManagerId == mgrId || lr.EmployeeId == mgrId);
            }
            else if (userRole != "Admin")
            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<LeaveRequestStatus>(status, true, out var parsedStatus))
                {
                    query = query.Where(lr => lr.Status == parsedStatus);
                }
            }

            var requests = await query
                .OrderByDescending(lr => lr.CreatedAt)
                .Select(lr => new LeaveRequestDTO
                {
                    Id = lr.Id,
                    EmployeeId = lr.EmployeeId,
                    EmployeeName = lr.Employee.FullName,
                    LeaveTypeId = lr.LeaveTypeId,
                    LeaveTypeName = lr.LeaveType.Name,
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    Reason = lr.Reason,
                    Status = lr.Status,
                    ApprovedById = lr.ApprovedById,
                    ApprovedByName = lr.ApprovedBy != null ? lr.ApprovedBy.FullName : null,
                    CreatedAt = lr.CreatedAt
                }).ToListAsync();

            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequestDTO>> GetById(int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;

            var lr = await _context.LeaveRequests
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .Include(r => r.ApprovedBy)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (lr == null) return NotFound();

            // Authorization check
            if (userRole == "Employee" && employeeIdClaim != lr.EmployeeId.ToString())
            {
                return Forbid();
            }
            else if (userRole == "Manager" && employeeIdClaim != lr.EmployeeId.ToString() && lr.Employee.ManagerId?.ToString() != employeeIdClaim)
            {
                return Forbid();
            }

            var dto = new LeaveRequestDTO
            {
                Id = lr.Id,
                EmployeeId = lr.EmployeeId,
                EmployeeName = lr.Employee.FullName,
                LeaveTypeId = lr.LeaveTypeId,
                LeaveTypeName = lr.LeaveType.Name,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Reason = lr.Reason,
                Status = lr.Status,
                ApprovedById = lr.ApprovedById,
                ApprovedByName = lr.ApprovedBy?.FullName,
                CreatedAt = lr.CreatedAt
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Apply([FromBody] LeaveRequestCreateDTO dto)
        {
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int empId))
            {
                return BadRequest("User profile does not contain employee information");
            }

            if (dto.StartDate > dto.EndDate)
            {
                return BadRequest("Start date cannot be after end date");
            }

            var leaveType = await _context.LeaveTypes.FindAsync(dto.LeaveTypeId);
            if (leaveType == null)
            {
                return BadRequest("Invalid leave type");
            }

            // 1. Check for overlapping requests (not rejected)
            var overlapExists = await _context.LeaveRequests
                .AnyAsync(lr => lr.EmployeeId == empId &&
                                lr.Status != LeaveRequestStatus.Rejected &&
                                lr.StartDate <= dto.EndDate &&
                                lr.EndDate >= dto.StartDate);

            if (overlapExists)
            {
                return BadRequest("Overlapping leave request already exists for these dates");
            }

            // 2. Validate leave balance
            int requestedDays = dto.EndDate.DayNumber - dto.StartDate.DayNumber + 1;
            int currentYear = dto.StartDate.Year;

            // Get total approved leave days of this type in the current year
            var approvedRequests = await _context.LeaveRequests
                .Where(lr => lr.EmployeeId == empId &&
                             lr.LeaveTypeId == dto.LeaveTypeId &&
                             lr.Status == LeaveRequestStatus.Approved &&
                             lr.StartDate.Year == currentYear)
                .ToListAsync();

            int approvedDays = approvedRequests.Sum(lr => lr.EndDate.DayNumber - lr.StartDate.DayNumber + 1);
            int remainingDays = leaveType.DefaultDaysPerYear - approvedDays;

            if (requestedDays > remainingDays)
            {
                return BadRequest($"Insufficient leave balance. Requested: {requestedDays} days, Remaining: {remainingDays} days");
            }

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = empId,
                LeaveTypeId = dto.LeaveTypeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Status = LeaveRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Leave request submitted successfully", requestId = leaveRequest.Id });
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}/decide")]
        public async Task<IActionResult> Decide(int id, [FromBody] LeaveRequestDecideDTO dto)
        {
            var deciderEmpIdClaim = User.FindFirst("employeeId")?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var leaveRequest = await _context.LeaveRequests
                .Include(lr => lr.Employee)
                .FirstOrDefaultAsync(lr => lr.Id == id);

            if (leaveRequest == null) return NotFound();

            if (leaveRequest.Status != LeaveRequestStatus.Pending)
            {
                return BadRequest("Decision has already been made for this request");
            }

            // Manager authorization check
            if (userRole == "Manager" && int.TryParse(deciderEmpIdClaim, out int mgrId))
            {
                if (leaveRequest.Employee.ManagerId != mgrId)
                {
                    return Forbid("You can only approve or reject leave requests from your direct reports");
                }
            }

            leaveRequest.Status = dto.Status;
            if (int.TryParse(deciderEmpIdClaim, out int deciderId))
            {
                leaveRequest.ApprovedById = deciderId;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Leave request successfully {dto.Status.ToString().ToLower()}" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var employeeIdClaim = User.FindFirst("employeeId")?.Value;
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null) return NotFound();

            if (leaveRequest.EmployeeId.ToString() != employeeIdClaim)
            {
                return Forbid("You can only cancel your own leave requests");
            }

            if (leaveRequest.Status != LeaveRequestStatus.Pending)
            {
                return BadRequest("Only pending leave requests can be cancelled");
            }

            // Instead of deleting, we can update status to Rejected or just delete the request.
            // Let's delete the request as cancellation.
            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Leave request cancelled successfully" });
        }
    }
}
