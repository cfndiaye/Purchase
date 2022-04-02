using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Purchase.Services.Contract;
using PurchaseShared.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Purchase.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        public AccountController(IConfiguration config, ILogger<AccountController> logger, IUserService userService)
        {
            _configuration = config;
            _logger = logger;
            _userService = userService;
        }
        // POST api/<AccountController>
        [HttpPost]
        public async Task<IActionResult> PostLogin([FromBody] LoginModel login)
        {
            IActionResult response = Unauthorized();
            var result = await AuthenticateUser(login);
            if(result)
            {
                _logger.LogInformation("user login in success.");
                var tokenString = GenerateJWT(login);
                return Ok( new { token = tokenString});
            }
            _logger.LogWarning("user login failed");
            return response;
        }

        private string GenerateJWT(LoginModel loginModel)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                    _configuration["Jwt:Issuer"],
                    null,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<bool> AuthenticateUser(LoginModel login)
        {
            return await _userService.AuthenticateUserAsync(login.UserName, login.Password);
        }
    }
}
