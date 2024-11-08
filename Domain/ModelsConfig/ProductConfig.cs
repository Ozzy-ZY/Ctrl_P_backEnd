using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.ModelsConfig
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasColumnType("nvarchar(50)");
            builder.Property(x => x.Description).IsRequired().HasColumnType("nvarchar(400)");
            builder.Property(x => x.Category).IsRequired().HasColumnType("nvarchar(50)");
            builder.HasMany(p=> p.CartItems)
                .WithOne(i=> i.Product)
                .HasForeignKey(i=>i.ProductId);
        }
    }
}