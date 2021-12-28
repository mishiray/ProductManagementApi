using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Super Admin")]
    public class RolesController : ControllerBase
    {
        private readonly IUserService userService;

        public RolesController(IUserService userService)
        {
            this.userService = userService;
        }


        [HttpPost("CreateRole")]
        public async ValueTask<IActionResult> CreateRoleAsync(string roleName)
        {
            try
            {
                if (!await userService.RoleManager.RoleExistsAsync(roleName))
                {
                    var result = await userService.RoleManager.CreateAsync(new IdentityRole(roleName));

                    if (result.Succeeded)
                    {
                        return Ok("Role created successfully");
                    }
                    else
                    {
                        return BadRequest(new { error = "The role has not been added successfully" });
                    }
                }
                
                return BadRequest(new { error = "Role already Exists" });
            }
            catch
            {
                throw;
            }
        }

        [HttpDelete("DeleteRole")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> DeleteRole(string roleName)
        {
            try
            {
                if (await userService.RoleManager.RoleExistsAsync(roleName) is false)
                {
                    return BadRequest(new { error = "Role does not Exist" });
                }

                var result = await userService.RoleManager.DeleteAsync(await userService.RoleManager.FindByNameAsync(roleName));

                if (result.Succeeded)
                {
                    return Ok("Role deleted successfully");
                }
                else
                {
                    return BadRequest(new { error = "Role was not deleted" });

                }

            }
            catch
            {
                throw;
            }
        }


        [HttpGet("ViewAllRoles")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> ViewAllRoles()
        {
            try
            {
                return Ok(await userService.RoleManager.Roles.ToListAsync());
            }
            catch
            {
                throw;
            }
        }



        [HttpPost("AssignUserToRole")]
        public async ValueTask<IActionResult> AssignUserToRole(string email)
        {
            try
            {
                var user = await userService.UserManager.FindByEmailAsync(email);
                if (user is null)
                {
                    throw new Exception("User Not Found");
                }

                if (!await userService.UserManager.IsInRoleAsync(user, user.RoleName))
                {
                    var result = await userService.UserManager.AddToRoleAsync(user, user.RoleName);
                    if (result.Succeeded)
                    {
                        return Ok(new { result = $"User has been added to role {user.RoleName}" });
                    }
                    else
                    {
                        return BadRequest(new { error = "User has not been added to role" });
                    }

                }
                return Ok("User already assigned to role");
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("GetUserRole")]
        public async ValueTask<IActionResult> GetUserRole(string email)
        {
            try
            {
                var user = await userService.UserManager.FindByEmailAsync(email);
                if (user is null)
                {
                    return BadRequest(new { error = "User Not Found" });
                }

                return Ok(await userService.UserManager.GetRolesAsync(user));
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("RemoveUserFromRole")]
        public async ValueTask<IActionResult> RemoveUserFromRole(string email, string roleName)
        {
            try
            {
                var user = await userService.UserManager.FindByEmailAsync(email);
                if (user is null)
                {
                    return BadRequest(new { error = "User Not Found" });
                }

                if (!await userService.RoleManager.RoleExistsAsync(roleName))
                {
                    return BadRequest(new { error = "Role does not Exist" });
                }

                var result = await userService.UserManager.RemoveFromRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    return Ok(new { result = "User has been removed from role" });
                }
                else
                {
                    return BadRequest(new { error = "User has not been removed from role" });
                }

            }
            catch
            {
                throw;
            }
        }



    }
}
