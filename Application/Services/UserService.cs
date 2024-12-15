using Domain.Models;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;

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

    public async Task<AppUser?> GetUserInformation(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        return user;
    }
}