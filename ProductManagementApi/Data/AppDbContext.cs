using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Data
{
    public class AppDbContext : IdentityDbContext<AspNetUser>
    {

        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

    }
}
