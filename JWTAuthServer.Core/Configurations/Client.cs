using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthServer.Core.Configurations
{
    public class Client
    {
        public string ClientId { get; set; } = null!;
        public string Secret { get; set; } = null!;
        public List<String> Audiences { get; set; } = new();
    }
}
