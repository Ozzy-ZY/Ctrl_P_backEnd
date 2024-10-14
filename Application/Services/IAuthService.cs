using Application.DTOs;
using Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IAuthService
    {
        public Task<Result> RegistrationAsync(AppUserRegisterationDto model, string role);

        public Task<Result> LoginAsync(AppUserLoginDto model);
    }
}