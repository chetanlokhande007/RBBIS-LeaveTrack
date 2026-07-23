using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Repositories.Interfaces
{
    public interface IHolidayRepository
    {
        Task<IEnumerable<Holiday>> GetAllHolidaysAsync();
        Task<Holiday?> GetHolidayByIdAsync(int id);
        Task<Holiday> CreateHolidayAsync(Holiday holiday);
        Task UpdateHolidayAsync(Holiday holiday);
        Task DeleteHolidayAsync(Holiday holiday);
    }
}
