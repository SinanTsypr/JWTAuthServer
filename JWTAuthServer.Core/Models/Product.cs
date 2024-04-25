using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthServer.Core.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public Decimal Price { get; set; }
        public int Stock { get; set; }
        public string UserId { get; set; } = null!;
    }
}
