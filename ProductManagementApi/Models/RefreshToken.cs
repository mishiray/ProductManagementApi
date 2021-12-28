using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Models
{
    public class RefreshToken : DbEntity
    {
        public string UserId { get; set; } 
        public string Token { get; set; }
        public string JwtId { get; set; } 
        public bool IsUsed { get; set; } 
        public bool IsRevoked { get; set; } 
        public DateTime ExpiryDate { get; set; } 

        [ForeignKey(nameof(UserId))]
        public AspNetUser User { get; set; }
    }
}
