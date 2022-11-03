using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthenticationController> logger
            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            //_jwtConfig = jwtConfig;
            _configuration = configuration;
            _logger = logger;
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
                    // add user to a role
                    await _userManager.AddToRoleAsync(user, "AppUser");
                    


                    var token = await GenerateJwtTokenAsync(user);

                    return Ok(new AuthResult()
                    {
                        Token = token,
                        Result = true,
                        Roles = new List<string>()
                        {
                            "AppUser"
                        }
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
                        var token = await GenerateJwtTokenAsync(user);
                        var roles = await _userManager.GetRolesAsync(user);
                        return Ok(new AuthResult()
                        {
                            Token = token,
                            Result = true,
                            Roles = new List<string>(roles)

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




        private async Task<string> GenerateJwtTokenAsync(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtConfig"]);
            var claims = await GetAllValidClaims(user);
            //token descreptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256)

            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
             
        }


        //get all valid claims for user
        private async Task<List<Claim>> GetAllValidClaims(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
            };

            //getting the claims that we have assigned to the user
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            //getting user role and add it to the claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach(var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));

                var role = await _roleManager.FindByNameAsync(userRole);
                if(role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach(var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }

    }
}
