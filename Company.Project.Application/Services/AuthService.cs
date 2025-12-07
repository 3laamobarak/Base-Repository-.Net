using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Company.Project.Application.Contracts;
using Company.Project.Application.DTO.Account;
using Company.Project.Application.Helper;
using Company.Project.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Company.Project.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
        }
        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if(await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };
            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username is already registered!" };
            
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                NID = model.NID
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                    errors += $"{error.Description},";
                return new AuthModel { Message = errors };
            }
            await _userManager.AddToRoleAsync(user, "User");
            var jwtToken = await CreateJwtToken(user);
            
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
            
            return new AuthModel
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                Email = user.Email,
                Username = user.UserName,
                Roles = new List<string> { "User" },
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };
        }

        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }
            var jwtToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            
            // user.RefreshTokens.RemoveAll(t => t.IsExpired || t.IsRevoked);

            var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
            string refreshTokenValue;
            if (activeRefreshToken != null)
            {
                refreshTokenValue = activeRefreshToken.Token;
            }
            else
            {
                var newRefreshToken = GenerateRefreshToken();
                user.RefreshTokens.Add(newRefreshToken);
                await _userManager.UpdateAsync(user);
                refreshTokenValue = newRefreshToken.Token;
            }

            return new AuthModel
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                Email = user.Email,
                Username = user.UserName,
                Roles = roles.ToList(),
                RefreshToken = refreshTokenValue,
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(7)
            };
        }

        public async Task<string> AddRoleAsync(AddRole model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";
            
            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";
            
            var result = await _userManager.AddToRoleAsync(user, model.Role);
            
            return result.Succeeded ? string.Empty : "Something went wrong";
        }
        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return new AuthModel { Message = "Invalid refresh token" };

            var refreshToken = user.RefreshTokens.FirstOrDefault(t => t.Token == token);

            if (refreshToken == null || !refreshToken.IsActive)
                return new AuthModel { Message = "Invalid or expired refresh token" };

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                Email = user.Email,
                Username = user.UserName,
                Roles = roles.ToList(),
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn
            };
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            
            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }.Union(userClaims).Union(roleClaims);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users
                .Include(u=>u.RefreshTokens)
                .FirstOrDefaultAsync(u=>u.RefreshTokens.Any(t=>t.Token==token));
            if (user == null)
                return false;
            var refreshToken = user.RefreshTokens.FirstOrDefault(t=>t.Token==token);
            if(refreshToken == null || !refreshToken.IsActive)
                return false;
            refreshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return true;
        }
        
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                CreatedOn = DateTime.UtcNow
            };
        }
         

    }
}
