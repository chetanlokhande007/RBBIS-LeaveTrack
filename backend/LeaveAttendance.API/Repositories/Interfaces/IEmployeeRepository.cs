using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<IEnumerable<Employee>> GetEmployeesByManagerIdAsync(int managerId);
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<bool> IsManagerOfEmployeeAsync(int managerId, int employeeId);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(Employee employee);
    }
}
