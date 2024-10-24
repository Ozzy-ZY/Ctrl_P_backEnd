using Application.DTOs.Mappers;
using Application.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.JsonWebTokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using Application.DTOs.AuthModels;
using Infrastructure.DataAccess;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<AppUser> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterResult> RegistrationAsync(AppUserRegisterationDto model, string role)
        {
            // check if user Exists
            // create a new AppUser
            // add the user's Roles
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                return new RegisterResult { Success = false, Message = "User Already Exists" };
            }

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = model.AsAppUser();

                if (!(await _userManager.CreateAsync(user, model.Password)).Succeeded)
                {
                    return new RegisterResult { Success = false, Message = "Error Creating User" };
                }

                var roleCreationResult = await _roleManager.CreateAsync(new IdentityRole<int>(role));
                if (!roleCreationResult.Succeeded)
                {
                    if (roleCreationResult.Errors.Contains(new IdentityErrorDescriber().DuplicateRoleName(role)))
                    {
                        transaction.Rollback();
                        return new RegisterResult { Success = false, Message = "Error Creating Role" };
                    }
                }

                await _unitOfWork.CommitAsync();
                await _userManager.AddToRoleAsync(user, role);

                return new RegisterResult { Success = true, Message = "Successful Registration" };
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                return new RegisterResult { Success = false, Message = $"Registration failed: {ex.Message}" };
            }
        }

    public async Task<LoginResult> LoginAsync(AppUserLoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            var loginResult = new LoginResult()
            {
                Success = false,
                Error = "Please Enter Valid Credentials"
            };

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return loginResult;
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
               new(ClaimTypes.Name, user.UserName),
               new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
            var token = GenerateToken(authClaims);
            loginResult.Token = token; 
            loginResult.Success = true;
            loginResult.Error = null;
            if (user.RefreshTokens!.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens!.FirstOrDefault(t => t.IsActive);
                loginResult.RefreshToken = activeRefreshToken!.Token;
                loginResult.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                loginResult.RefreshToken = refreshToken!.Token;
                loginResult.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens!.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return loginResult;
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
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),// should Always be there temporarily
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };
            var tokenHandler = new JsonWebTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return token;

            // --- Old way ---
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var token = tokenHandler.CreateToken(tokenDescriptor);
            //return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomArr = new byte[64];
            using (var randomGen = RandomNumberGenerator.Create())
            {
                randomGen.GetBytes(randomArr);
            }

            return new RefreshToken()
            {
                Token = Convert.ToBase64String(randomArr),
                ExpiresOn = DateTime.Now.AddMonths(int.Parse(_configuration["Jwt:RefreshTokenExpiryMonths"]!)),
                CreatedAt = DateTime.Now,
            };
        }
    }
}