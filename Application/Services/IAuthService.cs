using Application.DTOs.AuthModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IAuthService
    {
        public Task<RegisterResult> RegistrationAsync(AppUserRegisterationDto model, string role);

        public Task<LoginResult> LoginAsync(AppUserLoginDto model);

        public Task<LoginResult> RefreshTokenAsync(RequestTokenModel model);

        public Task<LoginResult> RevokeTokenAsync(RequestTokenModel model);
    }
}