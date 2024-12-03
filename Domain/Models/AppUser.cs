﻿using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class AppUser : IdentityUser<int>
    {
        public override string UserName { get; set; }

        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        

    }
}