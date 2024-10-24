﻿using Application.DTOs.AuthModels;
using Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

        [HttpPost("RegisterAdmin")]
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
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("RegisterUser")]
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
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("Login")]
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
                return Ok(result);
            }
            return BadRequest(result.Error);
        }
    }
}