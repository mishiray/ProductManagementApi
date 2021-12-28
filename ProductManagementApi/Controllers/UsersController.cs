using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementApi.Dtos;
using ProductManagementApi.Models;
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
    public class UsersController : ControllerBase
    {
        //private readonly IRepositoryService repositoryService;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        public UsersController(IUserService userService, IMapper mapper)
        {
            this.userService = userService;
            this.mapper = mapper;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateUser(AspNetUserModel model)
        {
            try
            {
                var user = mapper.Map<AspNetUser>(model);
                return Ok(await userService.AddAsync(user));
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpGet("AllUsers")]
        public async ValueTask<IActionResult> AllUsers()
        {
            try
            {
                return Ok(await userService.GetAspNetUsers());
            }
            catch
            {
                throw;
            }
        }


    }
}
