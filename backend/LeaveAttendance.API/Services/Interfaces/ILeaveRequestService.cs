using LeaveAttendance.API.DTOs;

namespace LeaveAttendance.API.Services.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<IEnumerable<LeaveRequestDTO>> GetAllLeaveRequestsAsync(string? userRole, string? employeeIdClaim, string? status);
        Task<LeaveRequestDTO> GetLeaveRequestByIdAsync(int id, string? userRole, string? employeeIdClaim);
        Task<object> ApplyForLeaveAsync(LeaveRequestCreateDTO dto, string? employeeIdClaim);
        Task<object> DecideLeaveRequestAsync(int id, LeaveRequestDecideDTO dto, string? userRole, string? deciderEmpIdClaim);
        Task<object> CancelLeaveRequestAsync(int id, string? employeeIdClaim);
    }
}
