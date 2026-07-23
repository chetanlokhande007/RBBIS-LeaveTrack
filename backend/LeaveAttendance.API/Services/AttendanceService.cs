using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Repositories.Interfaces;
using LeaveAttendance.API.Services.Interfaces;

namespace LeaveAttendance.API.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceService(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        private static int GetEmployeeId(string? employeeIdClaim)
        {
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int empId))
            {
                throw new ArgumentException("User profile does not contain valid employee information");
            }
            return empId;
        }

        public async Task<object> GetTodayStatusAsync(string? employeeIdClaim)
        {
            var empId = GetEmployeeId(employeeIdClaim);
            var today = DateOnly.FromDateTime(DateTime.Today);
            
            var attendance = await _attendanceRepository.GetAttendanceAsync(empId, today);

            if (attendance == null)
            {
                return new { checkedIn = false, checkInTime = (DateTime?)null, checkOutTime = (DateTime?)null };
            }

            return new
            {
                checkedIn = attendance.CheckInTime != null && attendance.CheckOutTime == null,
                checkInTime = attendance.CheckInTime,
                checkOutTime = attendance.CheckOutTime,
                status = attendance.Status.ToString()
            };
        }

        public async Task<object> CheckInAsync(string? employeeIdClaim)
        {
            var empId = GetEmployeeId(employeeIdClaim);
            var today = DateOnly.FromDateTime(DateTime.Today);
            
            var attendance = await _attendanceRepository.GetAttendanceAsync(empId, today);

            if (attendance != null)
            {
                if (attendance.CheckInTime != null)
                {
                    throw new InvalidOperationException("Already checked in today");
                }
                attendance.CheckInTime = DateTime.UtcNow;
                attendance.Status = AttendanceStatus.Present;
                await _attendanceRepository.UpdateAttendanceAsync(attendance);
            }
            else
            {
                attendance = new Attendance
                {
                    EmployeeId = empId,
                    Date = today,
                    CheckInTime = DateTime.UtcNow,
                    CheckOutTime = null,
                    Status = AttendanceStatus.Present
                };
                await _attendanceRepository.CreateAttendanceAsync(attendance);
            }

            return new { message = "Checked in successfully", checkInTime = attendance.CheckInTime };
        }

        public async Task<object> CheckOutAsync(string? employeeIdClaim)
        {
            var empId = GetEmployeeId(employeeIdClaim);
            var today = DateOnly.FromDateTime(DateTime.Today);
            
            var attendance = await _attendanceRepository.GetAttendanceAsync(empId, today);

            if (attendance == null || attendance.CheckInTime == null)
            {
                throw new InvalidOperationException("You must check in first before checking out");
            }

            if (attendance.CheckOutTime != null)
            {
                throw new InvalidOperationException("Already checked out today");
            }

            attendance.CheckOutTime = DateTime.UtcNow;
            await _attendanceRepository.UpdateAttendanceAsync(attendance);

            return new { message = "Checked out successfully", checkOutTime = attendance.CheckOutTime };
        }

        public async Task<IEnumerable<AttendanceDTO>> GetMyHistoryAsync(string? employeeIdClaim, string? month, string? year)
        {
            var empId = GetEmployeeId(employeeIdClaim);
            
            DateOnly? startDate = null;
            DateOnly? endDate = null;

            if (int.TryParse(month, out int m) && int.TryParse(year, out int y))
            {
                startDate = new DateOnly(y, m, 1);
                endDate = startDate.Value.AddMonths(1).AddDays(-1);
            }

            var history = await _attendanceRepository.GetHistoryAsync(empId, startDate, endDate);

            return history.Select(MapToDTO);
        }

        public async Task<IEnumerable<AttendanceDTO>> GetTeamAttendanceAsync(string? userRole, string? employeeIdClaim, string? date, string? department)
        {
            if (userRole != "Admin" && userRole != "Manager")
            {
                throw new UnauthorizedAccessException("You do not have permission to view team attendance.");
            }

            var targetDate = DateOnly.FromDateTime(DateTime.Today);
            if (!string.IsNullOrEmpty(date) && DateOnly.TryParse(date, out var parsedDate))
            {
                targetDate = parsedDate;
            }

            int? managerId = null;
            if (userRole == "Manager")
            {
                managerId = GetEmployeeId(employeeIdClaim);
            }

            var records = await _attendanceRepository.GetTeamAttendanceAsync(targetDate, managerId, department, userRole!);

            return records.Select(MapToDTO);
        }

        private static AttendanceDTO MapToDTO(Attendance a)
        {
            return new AttendanceDTO
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee?.FullName ?? string.Empty,
                Date = a.Date,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                Status = a.Status
            };
        }
    }
}
