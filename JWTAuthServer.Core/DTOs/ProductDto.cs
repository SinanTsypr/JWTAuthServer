using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthServer.Core.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public Decimal Price { get; set; }
        public string UserId { get; set; } = null!;
    }
}
