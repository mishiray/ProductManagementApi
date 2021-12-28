using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProductManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {

            using (var servicescope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = servicescope.ServiceProvider.GetService<AppDbContext>();
                if (!context.Products.Any())
                {
                    context.Products.AddRange(new Product()
                    {
                        Name = "Product 1",
                        Amount = 2500,
                        Quantity = "2",
                        Description = "This product is number 1",
                        IsActive = true

                    }, new Product()
                    {
                        Name = "Product 2",
                        Amount = 2000,
                        Quantity = "5",
                        Description = "This product is number 2",
                        IsActive = true

                    });
                    context.SaveChanges();
                    ;
                }
                
            }
        }
    }
}
