using Microsoft.AspNetCore.Identity;
using ProductManagementApi.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Models
{
    public class AspNetUser : IdentityUser
    {

        public override string Email { get; set; }

        public override string UserName { get; set; }


        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [Required]
        public string RoleName { get; set; }

        public string Address { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
