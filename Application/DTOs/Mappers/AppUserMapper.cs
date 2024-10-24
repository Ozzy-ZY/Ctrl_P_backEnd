using Application.DTOs.AuthModels;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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