using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductManagementApi.Configuration;
using ProductManagementApi.Interfaces;
using ProductManagementApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementApi.Services
{
    public interface IUserService
    {
        public HttpContext HttpContext { get; }

        public IRepositoryService Repository { get; }

        public SignInManager<AspNetUser> SignInManager { get; }
        public UserManager<AspNetUser> UserManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }


        Task<AspNetUser> AddAsync<T>(T entity) where T : AspNetUser;
        Task<AspNetUser> UpdateAsync<T>(T entity) where T : AspNetUser;
        Task<List<AspNetUser>> GetAspNetUsers();
        ValueTask<LoginResponse> Login(LoginRequest request);
        Task<LoginResponse> VerifyToken(TokenRequest tokenRequest);
    }
    public class UserService : IUserService
    {
        public HttpContext HttpContext { get; }

        public IRepositoryService Repository { get; }

        public SignInManager<AspNetUser> SignInManager { get; }
        public UserManager<AspNetUser> UserManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        private readonly JwtConfig _jwtConfig;

        private readonly TokenValidationParameters tokenValidationParameters;

        public UserService(SignInManager<AspNetUser> signInManager, UserManager<AspNetUser> userManager, RoleManager<IdentityRole> roleManager,
            IRepositoryService repository, IOptions<JwtConfig> options)
        {
            this.Repository = repository;
            this.SignInManager = signInManager;
            this.UserManager = userManager;
            this.RoleManager = roleManager;
            this._jwtConfig = options.Value;
        }
         public async Task<AspNetUser> AddAsync<T>(T entity) where T : AspNetUser
        {
            var username = entity.UserName ?? entity.Email;

            var identityUser = new AspNetUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                RoleName = entity.RoleName,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                UserName = username,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                Address = entity.Address
            };

            var result = await UserManager.CreateAsync(identityUser, entity.Password);

            if (result.Succeeded)
            {
                if (await RoleManager.FindByNameAsync(entity.RoleName) is null)
                {
                    throw new NotSupportedException("This role does not exist");
                }

                AspNetUser aspnetUser = new AspNetUser();
                if (entity.Email is null)
                {
                    aspnetUser = await UserManager.FindByNameAsync(entity.UserName);
                }

                else
                {
                    aspnetUser = await UserManager.FindByEmailAsync(entity.Email);
                }

                var addToRole = await UserManager.AddToRoleAsync(aspnetUser, aspnetUser.RoleName);

                if (!addToRole.Succeeded)
                {
                    throw new Exception(string.Join(",", addToRole.Errors.Select(c => c.Description)));
                }

                if (await Repository.appDbContext.Set<T>().FirstOrDefaultAsync(c => c.Email == aspnetUser.Email) is not null)
                {
                    return aspnetUser;
                }

                entity.Id = aspnetUser.Id;


                return aspnetUser;
            }

            throw new Exception(string.Join(",", result.Errors.Select(c => c.Description)));
        }

        public async Task<AspNetUser> UpdateAsync<T>(T entity) where T : AspNetUser
        {
            try
            {
                var _aspNetUser = await Repository.appDbContext.Users
                    .SingleOrDefaultAsync(c => c.Id == entity.Id);

                _aspNetUser.FirstName = entity.FirstName;
                _aspNetUser.LastName = entity.LastName;
                _aspNetUser.Email = entity.Email;
                _aspNetUser.PhoneNumber = entity.PhoneNumber;
                _aspNetUser.Address = entity.Address;
                _aspNetUser.RoleName = entity.RoleName;
                _aspNetUser.PasswordHash = UserManager.PasswordHasher.HashPassword(_aspNetUser, entity.Password); // need to add password to model.

                var result = await UserManager.UpdateAsync(_aspNetUser);

                return _aspNetUser;
            }
            catch
            {
                throw;
            }

        }


        public async Task<List<AspNetUser>> GetAspNetUsers()
        {
            return await UserManager.Users.ToListAsync();
        }

        public async ValueTask<LoginResponse> Login(LoginRequest request)
        {
            var user = await UserManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            if (await UserManager.CheckPasswordAsync(user, request.Password))
            {
                return await GenerateTokenAsync(user);
            }

            throw new UnauthorizedAccessException($"Login failed for user {request.Email}");
        }
        public string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async ValueTask<LoginResponse> GenerateTokenAsync(AspNetUser user, DateTime? expiry = default)
        {
            if (expiry is null)
            {
                expiry = DateTime.UtcNow.AddMinutes(20);
            }

            var key = Encoding.ASCII.GetBytes(_jwtConfig.SigningKey);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, $"{user.UserName}"),
                new Claim(ClaimTypes.Email, $"{user.Email}"),
                new Claim(ClaimTypes.Role, user.RoleName)
            };

            var role = await RoleManager.Roles.FirstOrDefaultAsync(c => c.Name == user.RoleName);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                IssuedAt = DateTime.UtcNow,
                Expires = expiry,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                IsRevoked = false,
                Token = RandomString(25) + Guid.NewGuid()
            };

            await Repository.AddAsync(refreshToken);


            return new LoginResponse
            {
                Token = tokenHandler.WriteToken(token),
                RoleName = user.RoleName,
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<LoginResponse> VerifyToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = jwtTokenHandler.ValidateToken(tokenRequest.Token, tokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return null;
                    }
                }

                var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expDate > DateTime.UtcNow)
                {
                    return new LoginResponse()
                    {
                        Errors = new List<string>() { "" },
                        Success = false
                    };
                }

                var storedRefreshToken = await Repository.appDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedRefreshToken == null)
                {
                    return new LoginResponse()
                    {
                        Errors = new List<string>() { "Refresh token doesnt exist" },
                        Success = false
                    };
                }

                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return new LoginResponse()
                    {
                        Errors = new List<string>() { "Token has expired, user needs to relogin" },
                        Success = false
                    };
                }

                if (storedRefreshToken.IsUsed)
                {
                    return new LoginResponse()
                    {
                        Errors = new List<string>() { "Token has been used" },
                        Success = false
                    };
                }

                if (storedRefreshToken.IsRevoked)
                {
                    return new LoginResponse()
                    {
                        Errors = new List<string>() { "token has been revoked" },
                        Success = false
                    };
                }

                var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedRefreshToken.JwtId != jti)
                {
                    return new LoginResponse()
                    {
                        Errors = new List<string>() { "the token doenst mateched the saved token" },
                        Success = false
                    };
                }

                storedRefreshToken.IsUsed = true;
                Repository.appDbContext.RefreshTokens.Update(storedRefreshToken);
                await Repository.appDbContext.SaveChangesAsync();

                var user = await UserManager.FindByIdAsync(storedRefreshToken.UserId);
                return await GenerateTokenAsync(user);
            }
            catch
            {
                throw;
            }
        }


        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }
    }
}
