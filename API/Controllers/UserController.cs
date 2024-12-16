using Application.Services;
using Domain.StaticData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserController: ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("User-Details/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserDetails(int userId)
    {
        var user = await _userService.GetUserInformation(userId);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet("Get-All-Users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersDashBoard();
        if(users == null)
            return NotFound();
        else return Ok(users);
    }
    [HttpPut("ToggleLockUser/{userId:int}")]
    [Authorize(Roles = StaticData.AdminRole)]
    public async Task<IActionResult> LockUser(int userId)
    {
        var result = await _userService.ToggleLockUser(userId);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }
}