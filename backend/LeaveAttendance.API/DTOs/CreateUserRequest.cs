namespace LeaveAttendance.API.DTOs
{
    public class CreateUserRequest
    {
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
