using BCrypt.Net;
using EventsAPI.Models;
using EventsAPI.Models.DTOs;
using EventsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(IAuthService authService, UserManager<IdentityUser> userManager)
        {
            this.authService = authService;
            _userManager = userManager;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (await authService.RegisterUser(user))
            {
                var roles = new List<string> { "user" };
                await authService.AssignRoles(user.Username, roles);


                return Ok(new { status = "success", message = "Registration Successful" });
            }

            return Ok(new { status = "fail", message = "Registration Failed" });

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(User user)
        {
            var result = await authService.Login(user);

            if (result == true)
            {
                var token = await authService.GenerateToken(user);
                return Ok(new { status = "success", message = "Login Successful", data = token });
            }
            return Ok(new { status = "fail", message = "Login Failed" });

        }

        [HttpGet("getuserid")]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserId(User user)
        {
            var userId = await authService.GetUserId(user);

            //return Ok(new { userId = userId });
            return Ok(new { UserId = userId });
        }
    }
}
