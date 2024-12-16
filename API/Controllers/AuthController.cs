using System.Security.Claims;
using Application.DTOs.AuthModels;
using Application.Services;
using Azure;
using Domain.StaticData;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<AppUserRegisterationDto> _validatorRegister;

        private readonly IValidator<AppUserLoginDto> _validatorLogin;

        public AuthController(IAuthService authService, IValidator<AppUserRegisterationDto> validator, IValidator<AppUserLoginDto> validatorLogin)
        {
            _authService = authService;
            _validatorRegister = validator;
            _validatorLogin = validatorLogin;
        }

        [HttpGet("External-Login")]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth");
            var props = new AuthenticationProperties()
            {
                RedirectUri = redirectUrl,
            };
            return Challenge(props, provider);
        }
        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            // Retrieve external authentication result
            var authenticateResult = 
                await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return Unauthorized();

            var claims = authenticateResult.Principal.Identities.First().Claims.ToArray();
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Find or create user in your system
            if (email == null || name == null)
            {
                return Unauthorized("Invalid Email and name");
            }
            var user = new AppUserRegisterationDto(email, email, email+name);
            var registerResult = await _authService.RegistrationAsync(user, StaticData.UserRole);

            // Generate your JWT token
            var loginResult = await _authService.LoginAsync(new AppUserLoginDto(email, email+name));

            return Ok(loginResult);
        }
        [HttpPost("RegisterAdmin")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<IActionResult> RegisterAdmin(AppUserRegisterationDto model)
        {
            var modelState = await _validatorRegister.ValidateAsync(model);
            if (!modelState.IsValid)
            {
                return BadRequest(modelState.Errors);
            }
            var result = await _authService.RegistrationAsync(model, "Admin");
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("RegisterUser")]
        [Authorize]
        public async Task<IActionResult> RegisterUser(AppUserRegisterationDto model)
        {
            var modelState = await _validatorRegister.ValidateAsync(model);
            if (!modelState.IsValid)
            {
                return BadRequest(modelState.Errors);
            }
            var result = await _authService.RegistrationAsync(model, "User");
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("Login")]
        [Authorize]
        public async Task<IActionResult> Login(AppUserLoginDto model)
        {
            var modelState = await _validatorLogin.ValidateAsync(model);
            if (!modelState.IsValid)
            {
                return BadRequest(modelState.Errors);
            }
            var result = await _authService.LoginAsync(model);
            if (result.Success)
            {
                if (!string.IsNullOrEmpty(result.RefreshToken))
                {
                    SetRefreshTokenToCookie(result.RefreshToken, result.RefreshTokenExpiration);
                }
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RequestTokenModel model)
        {
            model.RefreshToken = Uri.UnescapeDataString(model.RefreshToken);
            var result = await _authService.RefreshTokenAsync(model);
            if (result.Success)
            {
                if (!string.IsNullOrEmpty(result.RefreshToken))
                {
                    SetRefreshTokenToCookie(result.RefreshToken, result.RefreshTokenExpiration);
                }

                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPut("RevokeToken")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<IActionResult> RevokeToken(RequestTokenModel model)
        {
            model.RefreshToken = Uri.UnescapeDataString(model.RefreshToken);
            var result = await _authService.RevokeTokenAsync(model);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        private void SetRefreshTokenToCookie(string refreshToken, DateTime expiry)
        {
            var cookieOption = new CookieOptions()
            {
                HttpOnly = true,
                Expires = expiry.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOption);
        }
    }
}