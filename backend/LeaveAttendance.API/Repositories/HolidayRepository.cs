using LeaveAttendance.API.Data;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveAttendance.API.Repositories
{
    public class HolidayRepository : IHolidayRepository
    {
        private readonly LeaveTrackDbContext _context;

        public HolidayRepository(LeaveTrackDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Holiday>> GetAllHolidaysAsync()
        {
            return await _context.Holidays.OrderBy(h => h.Date).ToListAsync();
        }

        public async Task<Holiday?> GetHolidayByIdAsync(int id)
        {
            return await _context.Holidays.FindAsync(id);
        }

        public async Task<Holiday> CreateHolidayAsync(Holiday holiday)
        {
            _context.Holidays.Add(holiday);
            await _context.SaveChangesAsync();
            return holiday;
        }

        public async Task UpdateHolidayAsync(Holiday holiday)
        {
            _context.Holidays.Update(holiday);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHolidayAsync(Holiday holiday)
        {
            _context.Holidays.Remove(holiday);
            await _context.SaveChangesAsync();
        }
    }
}
