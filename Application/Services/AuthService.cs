using Application.DTOs;
using Application.DTOs.Mappers;
using Application.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<Result> RegistrationAsync(AppUserRegisterationDto model, string role)
        {
            // check if user Exists
            // create a new AppUser
            // add the user's Roles
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                return new Result { Success = false, Message = "User Already Exists" };
            }

            var user = model.AsAppUser();
            if (!(await _userManager.CreateAsync(user, model.Password)).Succeeded)
            {
                return new Result { Success = false, Message = "Error Creating User" };
            }

            var roleCreationResult = await _roleManager.CreateAsync(new IdentityRole<int>(role));
            if (!roleCreationResult.Succeeded)
            {
                if (!roleCreationResult.Errors.Any(e => e.Code == "RoleAlreadyExists"))
                {
                    return new Result { Success = false, Message = "Error Creating Role" };
                }
            }

            await _userManager.AddToRoleAsync(user, role);

            return new Result { Success = true, Message = "Successful Registration" };
        }

        public async Task<Result> LoginAsync(AppUserLoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new Result()
                {
                    Success = false,
                    Message = "Please Enter Valid Credentials"
                };
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            string token = GenerateToken(authClaims);
            return new Result()
            {
                Success = true,
                Message = token
            };
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            // configure based on Role
            var expiryMinutes = roleClaim switch
            {
                "Admin" => int.Parse(_configuration["Jwt:AdminExpiryMinutes"]!),
                "User" => int.Parse(_configuration["Jwt:UserExpiryMinutes"]!),
                _ => int.Parse(_configuration["Jwt:UserExpiryMinutes"]!)
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:ValidIssuer"],
                Audience = _configuration["Jwt:ValidAudience"],
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),// should Always be there temporarly
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}