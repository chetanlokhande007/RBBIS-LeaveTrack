using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<Attendance?> GetAttendanceAsync(int employeeId, DateOnly date);
        Task<IEnumerable<Attendance>> GetHistoryAsync(int employeeId, DateOnly? startDate, DateOnly? endDate);
        Task<IEnumerable<Attendance>> GetTeamAttendanceAsync(DateOnly date, int? managerId, string? department, string userRole);
        Task<Attendance> CreateAttendanceAsync(Attendance attendance);
        Task UpdateAttendanceAsync(Attendance attendance);
    }
}
