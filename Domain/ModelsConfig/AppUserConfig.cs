using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.ModelsConfig
{
    public class AppUserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(x => x.UserName).HasMaxLength(64).IsRequired();
            builder.HasIndex(x => x.UserName).IsUnique();
            builder.HasOne(u => u.Cart)
                .WithOne(c => c.AppUser)
                .HasForeignKey<Cart>(c => c.UserId);
            builder.HasMany(u=> u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);
        }
    }
}