using LeaveAttendance.API.Data;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveAttendance.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LeaveTrackDbContext _context;

        public UserRepository(LeaveTrackDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
