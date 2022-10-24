using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersManagement.Data;

namespace UsersManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<SetupController> _logger;

        public SetupController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<SetupController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();

            return Ok(roles);
        }

        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole(string name)
        {
            var role_exist = await _roleManager.RoleExistsAsync(name);
            if (!role_exist)
            {
                var role_result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (role_result.Succeeded)
                {
                    return Ok(new
                    {
                        result = $"The role {name} has been added successfully"
                    });
                }
                return BadRequest(role_result.Errors);
            }
            return BadRequest("Role exist already!");
        }

        [HttpDelete("DeleteRole")]
        public async Task<IActionResult> DeleteRole( string name)
        {
            var role_exist = await _roleManager.RoleExistsAsync(name);
            if (role_exist)
            {
                var role = await _roleManager.FindByNameAsync(name);
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return Ok(new
                    {
                        result = $"The role {name} is deleted"
                    });
                }
                return BadRequest(result.Errors);
            }
            return BadRequest("Role does not exist");
        }


        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }


        [HttpPost("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole( string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var roleExist = await _roleManager.RoleExistsAsync(role);
                if (roleExist)
                {
                    var result = await _userManager.AddToRoleAsync(user, role);
                    if (result.Succeeded)
                    {
                        return Ok(new
                        {
                            result = $"{role} role assigned to {user.UserName} successfully"
                        });
                    }
                    return BadRequest(new
                    {
                        result = $"{role} role does not assigned to {user.UserName} successfully"
                    });
                }
                return BadRequest(new
                {
                    result = $"{role} Role does not exist"
                });
            }
            return BadRequest(new
            {
                result = $"{email} does not match any user !"
            });
        }


        [HttpGet("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles( string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(roles);
            }
            return BadRequest(new
            {
                result = $"{email} does not match any user !"
            });

        }

        [HttpPost("RemoveUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole( string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user != null)
            {
                var roleExist = await _roleManager.RoleExistsAsync(role);
                if (roleExist)
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, role);
                    return Ok($"the role {role} is removed from {user.UserName}");
                }
                return BadRequest(new {
                result = $"the role {role} is not found"
                });
            }
            return BadRequest(new { 
                result =  "User not found"
            });
        }

    }
}
