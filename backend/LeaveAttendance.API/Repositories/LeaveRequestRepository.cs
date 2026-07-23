using LeaveAttendance.API.Data;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveAttendance.API.Repositories
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly LeaveTrackDbContext _context;

        public LeaveRequestRepository(LeaveTrackDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeaveRequest>> GetAllLeaveRequestsAsync(string? userRole, int empId, string? status)
        {
            IQueryable<LeaveRequest> query = _context.LeaveRequests
                .Include(lr => lr.Employee)
                .Include(lr => lr.LeaveType)
                .Include(lr => lr.ApprovedBy);

            if (userRole == "Employee")
            {
                query = query.Where(lr => lr.EmployeeId == empId);
            }
            else if (userRole == "Manager")
            {
                query = query.Where(lr => lr.Employee.ManagerId == empId || lr.EmployeeId == empId);
            }

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveRequestStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(lr => lr.Status == parsedStatus);
            }

            return await query.OrderByDescending(lr => lr.CreatedAt).ToListAsync();
        }

        public async Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .Include(r => r.ApprovedBy)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> HasOverlappingRequestAsync(int empId, DateOnly startDate, DateOnly endDate)
        {
            return await _context.LeaveRequests
                .AnyAsync(lr => lr.EmployeeId == empId &&
                                lr.Status != LeaveRequestStatus.Rejected &&
                                lr.StartDate <= endDate &&
                                lr.EndDate >= startDate);
        }

        public async Task<int> GetApprovedLeaveDaysForYearAsync(int empId, int leaveTypeId, int year)
        {
            var approvedRequests = await _context.LeaveRequests
                .Where(lr => lr.EmployeeId == empId &&
                             lr.LeaveTypeId == leaveTypeId &&
                             lr.Status == LeaveRequestStatus.Approved &&
                             lr.StartDate.Year == year)
                .ToListAsync();

            return approvedRequests.Sum(lr => lr.EndDate.DayNumber - lr.StartDate.DayNumber + 1);
        }

        public async Task<LeaveRequest> CreateLeaveRequestAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task UpdateLeaveRequestAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLeaveRequestAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Remove(request);
            await _context.SaveChangesAsync();
        }
    }
}
