using LeaveAttendance.API.Data;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveAttendance.API.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly LeaveTrackDbContext _context;

        public EmployeeRepository(LeaveTrackDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Manager)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByManagerIdAsync(int managerId)
        {
            return await _context.Employees
                .Where(e => e.ManagerId == managerId || e.Id == managerId)
                .Include(e => e.Manager)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> IsManagerOfEmployeeAsync(int managerId, int employeeId)
        {
            return await _context.Employees
                .AnyAsync(e => e.Id == employeeId && e.ManagerId == managerId);
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(Employee employee)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }
}
