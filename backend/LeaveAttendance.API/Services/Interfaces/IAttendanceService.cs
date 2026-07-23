using LeaveAttendance.API.DTOs;

namespace LeaveAttendance.API.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<object> GetTodayStatusAsync(string? employeeIdClaim);
        Task<object> CheckInAsync(string? employeeIdClaim);
        Task<object> CheckOutAsync(string? employeeIdClaim);
        Task<IEnumerable<AttendanceDTO>> GetMyHistoryAsync(string? employeeIdClaim, string? month, string? year);
        Task<IEnumerable<AttendanceDTO>> GetTeamAttendanceAsync(string? userRole, string? employeeIdClaim, string? date, string? department);
    }
}
