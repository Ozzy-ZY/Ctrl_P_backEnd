using Domain.Models;

namespace Application.DTOs.Mappers;

public static class UserInfoMapper
{
    public static UserInfoDTO AsUserInfoDto(this AppUser user)
    {
        return new UserInfoDTO()
        {
            DisplayName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
        };
    }

    public static UserInfoDashboardDTO AsUserInfoDashboardDto(this AppUser user)
    {
        if (user.Orders.FirstOrDefault() == null)
        {
            return new UserInfoDashboardDTO()
            {
                Id = user.Id,
                UserName = user.UserName,
                IsLockedOut = user.IsLockedOut,
                JoinDate = user.JoinDate,
            };
        }
        return new UserInfoDashboardDTO()
        {
            Id = user.Id,
            UserName = user.UserName,
            IsLockedOut = user.IsLockedOut,
            JoinDate = user.JoinDate,
            LastOrderId = user.Orders.FirstOrDefault()!.Id
        };
    }
}