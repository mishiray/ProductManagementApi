using AutoMapper;   
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementApi.Dtos;
using ProductManagementApi.Models;
using ProductManagementApi.Services;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace ProductManagementApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Super Admin, Admin")]
    public class ProductsController : ControllerBase
    {

        private readonly IRepositoryService repositoryService;
        private readonly IMapper mapper;

        public ProductsController(IRepositoryService repositoryService, IMapper mapper)
        {
            this.repositoryService = repositoryService;
            this.mapper = mapper;
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet("NewProductsSum")]
        public async Task<IActionResult> NewProductsSum()
        {
            try
            {
                return Ok(await repositoryService.appDbContext.Products.Where(p => p.CreatedAt >= DateTime.Now.AddDays(-7)).SumAsync(g => g.Amount));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet("AllDisabled")]
        public async Task<IActionResult> GetDisabledProducts()
        {
            try
            {
                return Ok(await repositoryService.appDbContext.Products.Where(p => p.IsActive == false).OrderByDescending(s => s.CreatedAt).ToListAsync());
            }
            catch (Exception)
            {
                throw;
            }
        }


        [Authorize(Roles = "Super Admin")]
        [HttpGet("All")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {

                return Ok(await repositoryService.appDbContext.Products.ToListAsync());
            }
            catch (Exception)
            {
                throw;
            }
        }



        [Authorize(Roles = "Super Admin")]
        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(string id)
        {
            try
            {
                
                var product = await repositoryService.appDbContext.Products
                                    .SingleOrDefaultAsync(c=> c.Id == id);
                if (product is null)
                {
                    return BadRequest(new { error = "This product does not exist" });

                }
                return Ok(product);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [Authorize(Roles = "Super Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> AddProduct(ProductModel model)
        {
            try
            {
                var product = mapper.Map<Product>(model);
                return Ok(await repositoryService.AddAsync(product));
            }
            catch(Exception)
            {
                throw;
            }
        }


        [Authorize(Roles = "Super Admin")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProduct(ProductModel model)
        {
            try
            {
                var _product = repositoryService.appDbContext.Products.First(l => l.Id == model.Id);
                if (_product is null)
                {
                    return BadRequest(new { error = "This product does not exist" });
                }
                var product = mapper.Map<ProductModel, Product>(model, _product);
                return Ok(await repositoryService.UpdateAsync(product));
            }
            catch
            {
                throw;
            }
        }

        [HttpPut("Disable")]
        public async ValueTask<IActionResult> Disable(string id)
        {
            try
            {

                var _product = await repositoryService.appDbContext.Products.FirstAsync(l => l.Id == id);
                if (_product is null)
                {
                    throw new Exception("Product does not exist.");
                }

                return Ok(await repositoryService.DisableAsync(_product));

            }
            catch
            {
                throw;
            }
        }


        [Authorize(Roles = "Super Admin")]
        [HttpDelete("Delete")]
        public async ValueTask<IActionResult> Delete(string id)
        {
            try
            {

                var _product = await repositoryService.appDbContext.Products.FirstAsync(l => l.Id == id);
                if (_product is null)
                {
                    throw new Exception("Product does not exist.");
                }

                return Ok(await repositoryService.DeleteAsync(_product));
                
            }
            catch
            {
                throw;
            }
        }
    }
}
