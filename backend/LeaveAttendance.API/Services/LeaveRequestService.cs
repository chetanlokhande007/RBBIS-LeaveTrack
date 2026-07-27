using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Repositories.Interfaces;
using LeaveAttendance.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using LeaveAttendance.API.Data;

namespace LeaveAttendance.API.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly LeaveTrackDbContext _dbContext;
        private readonly ILogger<LeaveRequestService> _logger;

        public LeaveRequestService(
            ILeaveRequestRepository leaveRequestRepository, 
            ILeaveTypeRepository leaveTypeRepository,
            LeaveTrackDbContext dbContext,
            ILogger<LeaveRequestService> logger)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        private static int GetEmployeeId(string? employeeIdClaim)
        {
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int empId))
            {
                throw new ArgumentException("User profile does not contain valid employee information");
            }
            return empId;
        }

        public async Task<IEnumerable<LeaveRequestDTO>> GetAllLeaveRequestsAsync(string? userRole, string? employeeIdClaim, string? status)
        {
            int empId = 0;
            if (userRole == "Employee" || userRole == "Manager")
            {
                empId = GetEmployeeId(employeeIdClaim);
            }
            else if (userRole != "Admin" && userRole != "HR")
            {
                throw new UnauthorizedAccessException("You are not authorized.");
            }

            var requests = await _leaveRequestRepository.GetAllLeaveRequestsAsync(userRole, empId, status);
            return requests.Select(MapToDTO);
        }

        public async Task<LeaveRequestDTO> GetLeaveRequestByIdAsync(int id, string? userRole, string? employeeIdClaim)
        {
            var lr = await _leaveRequestRepository.GetLeaveRequestByIdAsync(id);
            if (lr == null) throw new KeyNotFoundException("Leave request not found");

            if (userRole == "Employee" && employeeIdClaim != lr.EmployeeId.ToString())
            {
                throw new UnauthorizedAccessException("You are not authorized to view this request.");
            }
            if (userRole == "Manager" && employeeIdClaim != lr.EmployeeId.ToString() && lr.Employee.ManagerId?.ToString() != employeeIdClaim)
            {
                throw new UnauthorizedAccessException("You are not authorized to view this request.");
            }

            return MapToDTO(lr);
        }

        public async Task<object> ApplyForLeaveAsync(LeaveRequestCreateDTO dto, string? employeeIdClaim)
        {
            var empId = GetEmployeeId(employeeIdClaim);

            if (dto.StartDate > dto.EndDate)
            {
                throw new ArgumentException("Start date cannot be after end date.");
            }

            var leaveType = await _leaveTypeRepository.GetLeaveTypeByIdAsync(dto.LeaveTypeId);
            if (leaveType == null)
            {
                throw new ArgumentException("Invalid leave type.");
            }

            if (await _leaveRequestRepository.HasOverlappingRequestAsync(empId, dto.StartDate, dto.EndDate))
            {
                throw new InvalidOperationException("Overlapping leave request already exists for these dates.");
            }

            int requestedDays = dto.EndDate.DayNumber - dto.StartDate.DayNumber + 1;
            int approvedDays = await _leaveRequestRepository.GetApprovedLeaveDaysForYearAsync(empId, dto.LeaveTypeId, dto.StartDate.Year);
            int remainingDays = leaveType.DefaultDaysPerYear - approvedDays;

            if (requestedDays > remainingDays)
            {
                throw new InvalidOperationException($"Insufficient leave balance. Requested: {requestedDays} days, Remaining: {remainingDays} days.");
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

            var created = await _leaveRequestRepository.CreateLeaveRequestAsync(leaveRequest);

            return new { message = "Leave request submitted successfully", requestId = created.Id };
        }

        public async Task<object> DecideLeaveRequestAsync(int id, LeaveRequestDecideDTO dto, string? userRole, string? deciderEmpIdClaim)
        {
            var leaveRequest = await _leaveRequestRepository.GetLeaveRequestByIdAsync(id);
            if (leaveRequest == null) throw new KeyNotFoundException("Leave request not found.");

            if (leaveRequest.Status != LeaveRequestStatus.Pending)
            {
                throw new InvalidOperationException("Decision has already been made for this request.");
            }

            var mgrId = GetEmployeeId(deciderEmpIdClaim);

            if (userRole == "Manager")
            {
                if (leaveRequest.Employee.ManagerId != mgrId)
                {
                    _logger.LogWarning("Manager {ManagerId} attempted to decide leave #{LeaveId} for employee {EmployeeId} who is not their direct report.", mgrId, id, leaveRequest.EmployeeId);
                    throw new UnauthorizedAccessException("You can only approve or reject leave requests from your direct reports.");
                }
            }

            if (dto.Status == LeaveRequestStatus.Rejected && string.IsNullOrWhiteSpace(dto.Remarks))
            {
                throw new ArgumentException("Remarks are mandatory when rejecting a leave request.");
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                leaveRequest.Status = dto.Status;
                leaveRequest.ApprovedById = mgrId;

                var approvalHistory = new LeaveApproval
                {
                    LeaveRequestId = leaveRequest.Id,
                    ApproverId = mgrId,
                    Action = dto.Status,
                    Remarks = dto.Remarks,
                    ActionDate = DateTime.UtcNow
                };

                _dbContext.LeaveApprovals.Add(approvalHistory);
                await _leaveRequestRepository.UpdateLeaveRequestAsync(leaveRequest);
                
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Manager {ManagerId} {Action} Leave #{LeaveId}. Reason: {Remarks}", mgrId, dto.Status, id, dto.Remarks ?? "N/A");

                return new { message = $"Leave request successfully {dto.Status.ToString().ToLower()}" };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to process leave decision for Leave #{LeaveId}", id);
                throw new InvalidOperationException("An error occurred while processing the leave decision.", ex);
            }
        }

        public async Task<object> CancelLeaveRequestAsync(int id, string? employeeIdClaim)
        {
            var leaveRequest = await _leaveRequestRepository.GetLeaveRequestByIdAsync(id);
            if (leaveRequest == null) throw new KeyNotFoundException("Leave request not found.");

            if (leaveRequest.EmployeeId.ToString() != employeeIdClaim)
            {
                throw new UnauthorizedAccessException("You can only cancel your own leave requests.");
            }

            if (leaveRequest.Status != LeaveRequestStatus.Pending)
            {
                throw new InvalidOperationException("Only pending leave requests can be cancelled.");
            }

            await _leaveRequestRepository.DeleteLeaveRequestAsync(leaveRequest);

            return new { message = "Leave request cancelled successfully" };
        }

        private static LeaveRequestDTO MapToDTO(LeaveRequest lr)
        {
            return new LeaveRequestDTO
            {
                Id = lr.Id,
                EmployeeId = lr.EmployeeId,
                EmployeeName = lr.Employee?.FullName ?? string.Empty,
                LeaveTypeId = lr.LeaveTypeId,
                LeaveTypeName = lr.LeaveType?.Name ?? string.Empty,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Reason = lr.Reason,
                Status = lr.Status,
                ApprovedById = lr.ApprovedById,
                ApprovedByName = lr.ApprovedBy?.FullName,
                CreatedAt = lr.CreatedAt
            };
        }
    }
}
