using EventsAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventsAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration config;

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration config)
        {
            this.userManager = userManager;
            this.config = config;
        }

        public async Task<bool> RegisterUser(User user)
        {
            var identityUser = new IdentityUser
            {
                UserName = user.Username,
                Email = user.Username
            };

            //Creates this user with the password so it can do hasing on it
            var result = await userManager.CreateAsync(identityUser, user.Password);

            return result.Succeeded;
        }

        public async Task<bool> Login(User user)
        {
            //var identityUser = await userManager.FindByNameAsync(user.Username);
            var identityUser = await userManager.FindByEmailAsync(user.Username);

            if (identityUser == null)
            {
                return false;
            }

            //Checks the password that is stored for the user
            return await userManager.CheckPasswordAsync(identityUser, user.Password);
        }

        public async Task<string> GenerateToken(User user)
        {
            var identityUser = await userManager.FindByEmailAsync(user.Username);

            if (identityUser == null)
            {
                return null;
            }

            var userRoles = await userManager.GetRolesAsync(identityUser);

            // Create a list of claims
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Username)
                };


            if (userRoles != null && userRoles.Any())
            {
                claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWTConfig:Key").Value));

            var signingCreds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    issuer: config.GetSection("JWTConfig:Issuer").Value,
                    audience: config.GetSection("JWTConfig:Audience").Value,
                    signingCredentials: signingCreds
                );

            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            // Pause execution and allow attaching a debugger
            //System.Diagnostics.Debugger.Break();

            return token;
        }


        public async Task<bool> AssignRoles(string userName, IEnumerable<string> roles)
        {
            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return false;
            }

            var result = await userManager.AddToRolesAsync(user, roles);
            return result.Succeeded;
        }

        public async Task<string> GetUserId(User user)
        {
            var userTemp = await userManager.FindByEmailAsync(user.Username);

            if (userTemp != null)
            {
                return userTemp.Id;
            }
            return null;
        }
    }
}
