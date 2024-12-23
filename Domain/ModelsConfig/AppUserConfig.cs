﻿using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.ModelsConfig
{
    public class AppUserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasData(
                new AppUser()
                {
                    Id = 1,
                    Email = "Admin@gmail.com",
                    UserName = "Admin",
                    FirstName = "Admin",
                    LastName = "Admin",
                    PasswordHash = "AQAAAAIAAYagAAAAEI/Mz/MSiXslOgbOUm8Tk09JnoF8eNvXacynXqq5IcbhSWm2QvhOgm+xDY/URasS6g==",
                }
            );
            builder.Property(u => u.IsLockedOut).IsRequired();
            builder.Property(u => u.JoinDate).HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.UserName).HasMaxLength(64).IsRequired();
            builder.HasIndex(x => x.UserName).IsUnique();
            builder.HasOne(u => u.Cart)
                .WithOne(c => c.AppUser)
                .HasForeignKey<Cart>(c => c.UserId);
            builder.HasMany(u=> u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);
            builder.Property(u => u.FirstName).IsRequired();
            builder.Property(u => u.LastName).IsRequired();
        }
    }
}