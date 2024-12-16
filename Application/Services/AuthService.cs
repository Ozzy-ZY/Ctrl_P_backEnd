using Application.DTOs.Mappers;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using Application.DTOs.AuthModels;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Infrastructure.DataAccess.Repositories;

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
                var userCreation = await _userManager.CreateAsync(user, model.Password);
                if (!userCreation.Succeeded)
                {
                    transaction.Rollback();
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

                await _userManager.AddToRoleAsync(user, role);
                if (role == "User")
                {
                    await _unitOfWork.Carts.AddAsync(new Cart()
                        {
                            UserId = user.Id,
                            TotalPrice = 0,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            IsActive = true
                        });
                }

                await _unitOfWork.CommitAsync();
                transaction.Commit();
                return new RegisterResult { Success = true, Message = "Successful Registration" };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new RegisterResult { Success = false, Message = $"Registration failed: {ex.Message}" };
            }
        }

        public async Task<LoginResult> LoginAsync(AppUserLoginDto model)
        {
            var loginResult = new LoginResult()
            {
                Success = false,
                Error = "Please Enter Valid Credentials"
            };
            // used Eager Loading to prevent called the database multiple times for a single query
            // this fixes a memory Leakage that appeared when trying to log-in multiple times
            var user = await _userManager.Users
                .Include(user => user.RefreshTokens)
                .FirstOrDefaultAsync(user => user.UserName == model.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return loginResult;
            }

            if (user.IsLockedOut)
            {
                loginResult.Success = false;
                loginResult.Error = "Suspended Account!";
                return loginResult;
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
               new(ClaimTypes.Name, user.UserName),
               new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
            authClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            var token = GenerateAccessToken(authClaims);
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
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var refreshToken = GenerateRefreshToken();
                    loginResult.RefreshToken = refreshToken.Token;
                    loginResult.RefreshTokenExpiration = refreshToken.ExpiresOn;
                    user.RefreshTokens!.Add(refreshToken);
                    await _userManager.UpdateAsync(user);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new LoginResult()
                    {
                        Success = false,
                        Error = ex.Message,
                    };
                }
            }

            return loginResult;
        }

        public async Task<LoginResult> RevokeTokenAsync(RequestTokenModel model)
        {
            var logoutResult = new LoginResult();
            var user = await _userManager.Users
                .SingleOrDefaultAsync(
                u => u.RefreshTokens!
                .Any(t => t.Token == model.RefreshToken));
            if (user == null)
            {
                logoutResult.Error = "No user has this Refresh Token lol";
                logoutResult.Success = false;
                return logoutResult;
            }
            var refreshToken = user.RefreshTokens!.Single(t => t.Token == model.RefreshToken);
            if (!refreshToken.IsActive)
            {
                logoutResult.Error = "Already Revoked";
                logoutResult.Success = false;
                return logoutResult;
            }
            refreshToken.RevokedOn = DateTime.Now;
            if (!(await _userManager.UpdateAsync(user)).Succeeded)
            {
                logoutResult.Success = false;
                logoutResult.Error = "Problem with updating User";
                return logoutResult;
            }
            logoutResult.Success = true;
            return logoutResult;
        }

        public async Task<LoginResult> RefreshTokenAsync(RequestTokenModel model)
        {
            var loginResult = new LoginResult();
            var principal = GetPrincipalFromExpiredToken(model.Token);
            var username = principal.Identity!.Name;
            var user = await _userManager.FindByNameAsync(username!);
            if (user == null)
            {
                loginResult.Success = false;
                loginResult.Error = "Invalid Token!";
                return loginResult;
            }

            var refToken = user.RefreshTokens!.SingleOrDefault(t => t.Token == model.RefreshToken);
            if (refToken == null)
            {
                loginResult.Success = false;
                loginResult.Error = "Token is null";
                return loginResult;
            }
            if (!refToken.IsActive)
            {
                loginResult.Success = false;
                loginResult.Error = "InActive Token!";
                return loginResult;
            }
            loginResult.Token = GenerateAccessToken(principal.Claims);
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens!.Add(newRefreshToken);
            loginResult.Success = true;
            refToken.RevokedOn = DateTime.Now;
            loginResult.RefreshToken = newRefreshToken.Token;
            loginResult.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            loginResult.RefreshToken = refToken.Token;
            loginResult.RefreshTokenExpiration = refToken.ExpiresOn;
            await _userManager.UpdateAsync(user);
            return loginResult;
        }

        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var listOfClaims = claims.ToList();
            var roleClaim = listOfClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
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
                Subject = new ClaimsIdentity(listOfClaims)
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
                ExpiresOn = DateTime.Now.AddHours(int.Parse(_configuration["Jwt:RefreshTokenExpiryHours"]!)),
                CreatedAt = DateTime.Now,
            };
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                NameClaimType = ClaimTypes.NameIdentifier,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false, // Bypass expiration check
                ValidIssuer = _configuration["Jwt:ValidIssuer"],
                ValidAudience = _configuration["Jwt:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!))
            };

            ClaimsPrincipal? principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);

            return principal;
        }
    }
}