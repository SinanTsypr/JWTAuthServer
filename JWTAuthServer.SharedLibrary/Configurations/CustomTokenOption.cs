using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthServer.SharedLibrary.Configurations
{
    public class CustomTokenOption
    {
        public List<string> Audience { get; set; } = new();
        public string Issuer { get; set; } = null!;
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
        public string SecurityKey { get; set; } = null!;
    }
}
