using ProductManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Interfaces
{
    public interface IContact
    {
        public string ContactId { get; set; }
        public AspNetUser Contact { get; set; }
    }
}
