using LeaveAttendance.API.Data;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveAttendance.API.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly LeaveTrackDbContext _context;

        public AttendanceRepository(LeaveTrackDbContext context)
        {
            _context = context;
        }

        public async Task<Attendance?> GetAttendanceAsync(int employeeId, DateOnly date)
        {
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == date);
        }

        public async Task<IEnumerable<Attendance>> GetHistoryAsync(int employeeId, DateOnly? startDate, DateOnly? endDate)
        {
            IQueryable<Attendance> query = _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.EmployeeId == employeeId);

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(a => a.Date >= startDate.Value && a.Date <= endDate.Value);
            }

            return await query
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetTeamAttendanceAsync(DateOnly date, int? managerId, string? department, string userRole)
        {
            IQueryable<Attendance> query = _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.Date == date);

            if (userRole == "Manager" && managerId.HasValue)
            {
                query = query.Where(a => a.Employee.ManagerId == managerId.Value);
            }

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(a => a.Employee.Department.ToLower() == department.ToLower());
            }

            return await query.ToListAsync();
        }

        public async Task<Attendance> CreateAttendanceAsync(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task UpdateAttendanceAsync(Attendance attendance)
        {
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
        }
    }
}
