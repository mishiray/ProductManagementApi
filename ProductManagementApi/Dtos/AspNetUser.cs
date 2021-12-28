using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Dtos
{
    public class AspNetUserModel
    {

        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
        public string Address { get; set; }

    }
}
