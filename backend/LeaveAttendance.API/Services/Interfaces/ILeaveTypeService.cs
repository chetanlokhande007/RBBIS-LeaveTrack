using LeaveAttendance.API.DTOs;

namespace LeaveAttendance.API.Services.Interfaces
{
    public interface ILeaveTypeService
    {
        Task<IEnumerable<LeaveTypeDTO>> GetAllLeaveTypesAsync();
        Task<LeaveTypeDTO> GetLeaveTypeByIdAsync(int id);
        Task<LeaveTypeDTO> CreateLeaveTypeAsync(LeaveTypeDTO dto);
        Task UpdateLeaveTypeAsync(int id, LeaveTypeDTO dto);
        Task DeleteLeaveTypeAsync(int id);
    }
}
