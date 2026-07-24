using LeaveAttendance.API.DTOs;

namespace LeaveAttendance.API.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync(string? userRole, string? employeeIdClaim);
        Task<(IEnumerable<EmployeeDTO> Items, int TotalCount)> GetPagedEmployeesAsync(int page, int pageSize, string? search, string? department, string? designation, string? userRole, string? employeeIdClaim);
        Task<EmployeeDTO> GetEmployeeByIdAsync(int id, string? userRole, string? employeeIdClaim);
        Task<EmployeeDTO> CreateEmployeeAsync(EmployeeCreateUpdateDTO dto);
        Task UpdateEmployeeAsync(int id, EmployeeCreateUpdateDTO dto);
        Task DeleteEmployeeAsync(int id);
    }
}
