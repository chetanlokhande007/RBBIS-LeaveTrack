using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeaveAttendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateUserAsync(request);
        return Ok(user);
    }
}