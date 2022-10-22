using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UsersManagement.Configurations;
using UsersManagement.Models;
using UsersManagement.Models.DTOs;

namespace UsersManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        //  private readonly JwtConfig _jwtConfig;
        private readonly IConfiguration _configuration;
        public AuthenticationController(UserManager<IdentityUser> userManager,IConfiguration configuration)
        {
            _userManager = userManager;
            //_jwtConfig = jwtConfig;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestDto)
        {
            if (ModelState.IsValid)
            {
                var email_exist = await _userManager.FindByEmailAsync(requestDto.Email);
                if(email_exist != null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Email already exist"
                        }
                    });
                }

                //create user
                var user = new IdentityUser
                {
                    Email = requestDto.Email,
                    UserName = requestDto.Name,
                };
                var result = await _userManager.CreateAsync(user,requestDto.Password);
                if (result.Succeeded)
                {
                    var token = GenerateJwtToken(user);
                    return Ok(new AuthResult()
                    {
                        Token = token,
                        Result = true,

                    });


                }
                return BadRequest(new AuthResult()
                {
                    Result=false,
                    Errors = new List<string>()
                    {
                        "Server error"
                    }
                });
            }
            return BadRequest();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginRequestDto requestDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(requestDto.Email);
                if (user != null)
                {
                    var validPassword = await _userManager.CheckPasswordAsync(user, requestDto.Password);
                    if (validPassword)
                    {
                        var token = GenerateJwtToken(user);

                        return Ok(new AuthResult()
                        {
                            Token = token,
                            Result = true,
                        });
                    }
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Wrong Password!"
                        }
                    });
                }
                return BadRequest(new AuthResult()
                {
                    Result =false,
                    Errors = new List<string>()
                        {
                            "No account match with this email"
                        }
                });

            }
            return BadRequest();
        }




        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtConfig"]);

            //token descreptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
                }),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256)

            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
             
        }

    }
}
