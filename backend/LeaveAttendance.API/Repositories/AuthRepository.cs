using LeaveAttendance.API.Data;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveAttendance.API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly LeaveTrackDbContext _context;

        public AuthRepository(LeaveTrackDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User?> GetUserByResetTokenAsync(string token)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users
                .AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }

        public async Task CreateUserWithEmployeeAsync(User user, Employee? employee)
        {
            // If we are creating an employee alongside the user
            if (employee != null)
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                
                user.EmployeeId = employee.Id;
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
