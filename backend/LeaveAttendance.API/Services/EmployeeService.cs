using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Repositories.Interfaces;
using LeaveAttendance.API.Services.Interfaces;

namespace LeaveAttendance.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync(string? userRole, string? employeeIdClaim)
        {
            if (userRole == "Admin" || userRole == "HR")
            {
                var employees = await _employeeRepository.GetAllEmployeesAsync();
                return employees.Select(MapToDTO);
            }
            

            if (userRole == "Manager" && int.TryParse(employeeIdClaim, out int managerId))
            {
                var reports = await _employeeRepository.GetEmployeesByManagerIdAsync(managerId);
                return reports.Select(MapToDTO);
            }
            
            if (userRole == "Employee" && int.TryParse(employeeIdClaim, out int empId))
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(empId);
                if (employee == null) throw new KeyNotFoundException("Employee not found.");
                
                return new[] { MapToDTO(employee) };
            }

            throw new UnauthorizedAccessException("You are not authorized to view this resource.");
        }

        public async Task<(IEnumerable<EmployeeDTO> Items, int TotalCount)> GetPagedEmployeesAsync(int page, int pageSize, string? search, string? department, string? designation, string? userRole, string? employeeIdClaim)
        {
            if (userRole == "Admin" || userRole == "HR")
            {
                var (items, total) = await _employeeRepository.GetPagedEmployeesAsync(page, pageSize, search, department, designation);
                return (items.Select(MapToDTO), total);
            }
            throw new UnauthorizedAccessException("You are not authorized to view this resource.");
        }

        public async Task<EmployeeDTO> GetEmployeeByIdAsync(int id, string? userRole, string? employeeIdClaim)
        {
            if (userRole == "Employee" && employeeIdClaim != id.ToString())
            {
                throw new UnauthorizedAccessException("You can only view your own profile.");
            }

            if (userRole == "Manager" && employeeIdClaim != id.ToString())
            {
                int managerId = int.Parse(employeeIdClaim!);
                var isReport = await _employeeRepository.IsManagerOfEmployeeAsync(managerId, id);
                if (!isReport) throw new UnauthorizedAccessException("You are not authorized to view this employee's profile.");
            }

            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employee == null) throw new KeyNotFoundException("Employee not found.");

            return MapToDTO(employee);
        }

        public async Task<EmployeeDTO> CreateEmployeeAsync(EmployeeCreateUpdateDTO dto)
        {
            if (dto == null) throw new ArgumentException("Invalid request data.");

            var employee = new Employee
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Department = dto.Department,
                Designation = dto.Designation,
                ManagerId = dto.ManagerId,
                DateOfJoining = dto.DateOfJoining
            };

            var createdEmployee = await _employeeRepository.CreateEmployeeAsync(employee);

            // Fetch again to include manager name in DTO if ManagerId was provided
            if (createdEmployee.ManagerId.HasValue)
            {
                createdEmployee = await _employeeRepository.GetEmployeeByIdAsync(createdEmployee.Id) ?? createdEmployee;
            }

            return MapToDTO(createdEmployee);
        }

        public async Task UpdateEmployeeAsync(int id, EmployeeCreateUpdateDTO dto)
        {
            if (dto == null) throw new ArgumentException("Invalid request data.");

            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employee == null) throw new KeyNotFoundException("Employee not found.");

            employee.FullName = dto.FullName;
            employee.Email = dto.Email;
            employee.Department = dto.Department;
            employee.Designation = dto.Designation;
            employee.ManagerId = dto.ManagerId;
            employee.DateOfJoining = dto.DateOfJoining;

            await _employeeRepository.UpdateEmployeeAsync(employee);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employee == null) throw new KeyNotFoundException("Employee not found.");

            await _employeeRepository.DeleteEmployeeAsync(employee);
        }

        private static EmployeeDTO MapToDTO(Employee employee)
        {
            return new EmployeeDTO
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Department = employee.Department,
                Designation = employee.Designation,
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager?.FullName,
                DateOfJoining = employee.DateOfJoining
            };
        }
    }
}
