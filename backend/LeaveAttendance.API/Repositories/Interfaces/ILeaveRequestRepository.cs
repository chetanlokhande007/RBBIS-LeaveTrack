using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Repositories.Interfaces
{
    public interface ILeaveRequestRepository
    {
        Task<IEnumerable<LeaveRequest>> GetAllLeaveRequestsAsync(string? userRole, int empId, string? status);
        Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id);
        Task<bool> HasOverlappingRequestAsync(int empId, DateOnly startDate, DateOnly endDate);
        Task<int> GetApprovedLeaveDaysForYearAsync(int empId, int leaveTypeId, int year);
        Task<LeaveRequest> CreateLeaveRequestAsync(LeaveRequest request);
        Task UpdateLeaveRequestAsync(LeaveRequest request);
        Task DeleteLeaveRequestAsync(LeaveRequest request);
    }
}
