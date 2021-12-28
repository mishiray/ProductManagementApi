using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductManagementApi.Configuration;
using ProductManagementApi.Data;
using ProductManagementApi.Models;
using ProductManagementApi.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;

        public AuthController(IUserService userService)
        {
            this.userService = userService;
        }



        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var result = await userService.Login(request);
                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }
    
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            string id = HttpContext.User.FindFirstValue("nameid");
            if (!Guid.TryParse(id, out Guid userId))
            {
                return Unauthorized();
            }
            var user = await userService.UserManager.FindByIdAsync(id);

            return Ok(user);
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (ModelState.IsValid)
            {
                var res = await VerifyToken(tokenRequest);

                if (res == null)
                {
                    return BadRequest(new {
                        Errors = "Invalid Tokens" });
                }

                return Ok(res);
            }

            return BadRequest(new { Errors = "Invalid payload"});
        }

        private async Task<LoginResponse> VerifyToken(TokenRequest tokenRequest)
        {
            
            try
            {
                return await userService.VerifyToken(tokenRequest);
            }
            catch
            {
                return null;
            }
        }


    }
}
