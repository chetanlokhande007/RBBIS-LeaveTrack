using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Repositories.Interfaces;
using LeaveAttendance.API.Services.Interfaces;

namespace LeaveAttendance.API.Services
{
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly ILeaveTypeRepository _leaveTypeRepository;

        public LeaveTypeService(ILeaveTypeRepository leaveTypeRepository)
        {
            _leaveTypeRepository = leaveTypeRepository;
        }

        public async Task<IEnumerable<LeaveTypeDTO>> GetAllLeaveTypesAsync()
        {
            var types = await _leaveTypeRepository.GetAllLeaveTypesAsync();
            return types.Select(MapToDTO);
        }

        public async Task<LeaveTypeDTO> GetLeaveTypeByIdAsync(int id)
        {
            var type = await _leaveTypeRepository.GetLeaveTypeByIdAsync(id);
            if (type == null) throw new KeyNotFoundException("Leave type not found");

            return MapToDTO(type);
        }

        public async Task<LeaveTypeDTO> CreateLeaveTypeAsync(LeaveTypeDTO dto)
        {
            if (dto == null) throw new ArgumentException("Invalid request data");

            var leaveType = new LeaveType
            {
                Name = dto.Name,
                DefaultDaysPerYear = dto.DefaultDaysPerYear
            };

            var created = await _leaveTypeRepository.CreateLeaveTypeAsync(leaveType);
            return MapToDTO(created);
        }

        public async Task UpdateLeaveTypeAsync(int id, LeaveTypeDTO dto)
        {
            if (dto == null || id != dto.Id) throw new ArgumentException("Invalid request data");

            var type = await _leaveTypeRepository.GetLeaveTypeByIdAsync(id);
            if (type == null) throw new KeyNotFoundException("Leave type not found");

            type.Name = dto.Name;
            type.DefaultDaysPerYear = dto.DefaultDaysPerYear;

            await _leaveTypeRepository.UpdateLeaveTypeAsync(type);
        }

        public async Task DeleteLeaveTypeAsync(int id)
        {
            var type = await _leaveTypeRepository.GetLeaveTypeByIdAsync(id);
            if (type == null) throw new KeyNotFoundException("Leave type not found");

            await _leaveTypeRepository.DeleteLeaveTypeAsync(type);
        }

        private static LeaveTypeDTO MapToDTO(LeaveType lt)
        {
            return new LeaveTypeDTO
            {
                Id = lt.Id,
                Name = lt.Name,
                DefaultDaysPerYear = lt.DefaultDaysPerYear
            };
        }
    }
}
