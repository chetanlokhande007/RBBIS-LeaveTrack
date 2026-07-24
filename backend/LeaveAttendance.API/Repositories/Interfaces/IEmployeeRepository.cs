using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<(IEnumerable<Employee> Items, int TotalCount)> GetPagedEmployeesAsync(int page, int pageSize, string? search, string? department, string? designation);
        Task<IEnumerable<Employee>> GetEmployeesByManagerIdAsync(int managerId);
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<bool> IsManagerOfEmployeeAsync(int managerId, int employeeId);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(Employee employee);
    }
}
