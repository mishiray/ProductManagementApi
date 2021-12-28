using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Dtos
{
    public class ProductModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string Quantity { get; set; }
        public string Description { get; set; }
    }
}
