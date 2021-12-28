using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Models
{
    public class Product : DbEntity
    {
        [Required]
        public string Name { get; set; }
        public double Amount { get; set; }
        public string Quantity { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
