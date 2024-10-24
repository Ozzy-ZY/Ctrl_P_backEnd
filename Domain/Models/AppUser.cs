﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class AppUser : IdentityUser<int>
    {
        public override string UserName { get; set; }

        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }
    }
}