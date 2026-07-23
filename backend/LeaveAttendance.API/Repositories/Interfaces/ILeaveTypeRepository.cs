using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Repositories.Interfaces
{
    public interface ILeaveTypeRepository
    {
        Task<IEnumerable<LeaveType>> GetAllLeaveTypesAsync();
        Task<LeaveType?> GetLeaveTypeByIdAsync(int id);
        Task<LeaveType> CreateLeaveTypeAsync(LeaveType leaveType);
        Task UpdateLeaveTypeAsync(LeaveType leaveType);
        Task DeleteLeaveTypeAsync(LeaveType leaveType);
    }
}
