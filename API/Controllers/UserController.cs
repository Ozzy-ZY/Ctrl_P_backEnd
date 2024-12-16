using System.Security.Claims;
using Application.DTOs;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LockUser(int userId)
    {
        var result = await _userService.ToggleLockUser(userId);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPut("Update-User-Info/")]
    [Authorize(Roles = StaticData.UserRole)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UserInfoDTO userInfo)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var result = await _userService.UpdateUserInformation(userInfo,userId);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut("Change-Password/")]
    [Authorize(Roles = StaticData.UserRole)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var result = await _userService.UpdateUserPassword(changePasswordDto, userId);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}