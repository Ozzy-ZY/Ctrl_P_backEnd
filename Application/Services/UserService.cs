using Application.DTOs;
using Application.DTOs.Mappers;
using Domain.Models;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public UserService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<UserInfoDTO?> GetUserInformation(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        var info = user?.AsUserInfoDto();
        return info;
    }

    public Task<IEnumerable<UserInfoDashboardDTO>?> GetAllUsersDashBoard()
    {
        var users = _userManager.Users.Include(u=> u.Orders);
        List<UserInfoDashboardDTO> usersDashboard = new List<UserInfoDashboardDTO>();
        foreach (var user in users)
        {
            usersDashboard.Add(user.AsUserInfoDashboardDto());
        }
        return Task.FromResult<IEnumerable<UserInfoDashboardDTO>?>(usersDashboard);
    }

    public async Task<ServiceResult> ToggleLockUser(int userId)
    {
        var result = new ServiceResult()
        {
            Success = false
        };
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            result.Errors.Add("User not found");
            return result;
        }

        if (user.IsLockedOut == false)
        {
            user.IsLockedOut = true;
            user.RefreshTokens?.Clear();
        }
        else if (user.IsLockedOut == true)
            user.IsLockedOut = false;
        var idResult = await _userManager.UpdateAsync(user);
        if (idResult.Succeeded)
        {
            result.Success = true;
            return result;
        }
        result.Errors.Add("Toggle Lockout Failed");
        return result;
    }
}