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

        public async Task<(IEnumerable<Employee> Items, int TotalCount)> GetPagedEmployeesAsync(int page, int pageSize, string? search, string? department, string? designation)
        {
            var query = _context.Employees.Include(e => e.Manager).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(e => e.FullName.ToLower().Contains(searchLower) || e.Email.ToLower().Contains(searchLower));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(e => e.Department == department);
            }

            if (!string.IsNullOrWhiteSpace(designation))
            {
                query = query.Where(e => e.Designation == designation);
            }

            var totalCount = await query.CountAsync();
            
            var items = await query
                .OrderBy(e => e.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
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
