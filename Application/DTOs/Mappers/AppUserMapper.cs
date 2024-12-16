using Application.DTOs.AuthModels;
using Domain.Models;

namespace Application.DTOs.Mappers
{
    public static class AppUserMapper
    {
        public static AppUser AsAppUser(this AppUserRegisterationDto dto)
        {
            return new AppUser()
            {
                Email = dto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = dto.UserName,
                FirstName = dto.UserName,// just to be used when first Registered
                LastName = dto.UserName, // just to be used when first Registered
                IsLockedOut = false,
            };
        }

        public static void AsAppUser(this UserInfoDTO userInfo, ref AppUser appUser)
        {
            appUser.FirstName = userInfo.FirstName;
            appUser.LastName = userInfo.LastName;
            appUser.Email = userInfo.Email;
        }
    }
}