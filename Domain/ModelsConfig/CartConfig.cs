using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.ModelsConfig;

public class CartConfig:IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");
        builder.HasKey(x => x.Id);
        builder.HasMany(c=> c.CartItems)
            .WithOne(i => i.Cart)
            .HasForeignKey(i => i.CartId);
        builder.Property(c => c.TotalPrice)
    .HasPrecision(18, 2); // Example: Precision 18, scale 2

    }
}