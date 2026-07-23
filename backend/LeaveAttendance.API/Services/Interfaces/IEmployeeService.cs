using LeaveAttendance.API.DTOs;

namespace LeaveAttendance.API.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync(string? userRole, string? employeeIdClaim);
        Task<EmployeeDTO> GetEmployeeByIdAsync(int id, string? userRole, string? employeeIdClaim);
        Task<EmployeeDTO> CreateEmployeeAsync(EmployeeCreateUpdateDTO dto);
        Task UpdateEmployeeAsync(int id, EmployeeCreateUpdateDTO dto);
        Task DeleteEmployeeAsync(int id);
    }
}
