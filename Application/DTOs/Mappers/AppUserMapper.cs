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
            };
        }
    }
}