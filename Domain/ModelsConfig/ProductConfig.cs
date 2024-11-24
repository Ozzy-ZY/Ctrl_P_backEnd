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
            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("nvarchar(50)");
            builder.Property(x => x.Description)
                .IsRequired()
                .HasColumnType("nvarchar(400)");
            builder.Property(x => x.Price)
                .HasPrecision(18, 2); // Precision 18, scale 2 (e.g., 1234567890123456.78)
            builder.Property(x => x.OldPrice)
                .HasPrecision(18, 2);
            builder.Property(x => x.Rating)
                .HasPrecision(2, 1) // Allows max value of 5.0 (1 digit before decimal, 1 digit after)
                .HasDefaultValue(0); // Optional: default rating value
            builder.HasMany(p => p.CartItems)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId);
        }
    }

}