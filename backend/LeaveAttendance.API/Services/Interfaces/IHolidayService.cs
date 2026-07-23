using LeaveAttendance.API.DTOs;

namespace LeaveAttendance.API.Services.Interfaces
{
    public interface IHolidayService
    {
        Task<IEnumerable<HolidayDTO>> GetAllHolidaysAsync();
        Task<HolidayDTO> GetHolidayByIdAsync(int id);
        Task<HolidayDTO> CreateHolidayAsync(HolidayDTO dto);
        Task UpdateHolidayAsync(int id, HolidayDTO dto);
        Task DeleteHolidayAsync(int id);
    }
}
