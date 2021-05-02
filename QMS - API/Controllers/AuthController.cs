using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QMS__API.Resources;
using QMS_API.Models;

namespace QMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppSettings _appSettings;



        public AuthController(UserManager<ApplicationUser> userManager, IOptionsSnapshot<AppSettings> options)
        {
            _userManager = userManager;
            _appSettings = options.Value;
        }


        [HttpPost]
        [Route("register")]
        //POST : /api/ApplicationUser/Register
        public async Task<IActionResult> PostApplicationUser([FromBody] UserResource model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var applicationUser = new ApplicationUser()
            {
                UserName = model.Name,
                Email = model.Email
            };

            try
            {
                var result = await _userManager.CreateAsync(applicationUser, model.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserResource model)
        {
            var user = await _userManager.FindByNameAsync(model.Name);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("UserID", user.Id),
                        new Claim("UserName", user.UserName)
                    }),

                    Expires = DateTime.UtcNow.AddDays(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_SecretKey)), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);

                return Ok(new {token});
            }
            else
            {
                return BadRequest(new { message = "UserName or Password Incorrect!" });
            }

        }
    }
}
