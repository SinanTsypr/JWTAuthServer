﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthServer.Core.Models
{
    public class UserApp : IdentityUser
    {
        public string City { get; set; } = null!;
    }
}
