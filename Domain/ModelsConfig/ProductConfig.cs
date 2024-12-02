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
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasColumnType("nvarchar(400)")
                .HasMaxLength(400);

            builder.Property(x => x.Price)
                .HasPrecision(18, 2)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.OldPrice)
                .HasPrecision(18, 2);

            builder.Property(x => x.Rating)
                .HasPrecision(2, 1)
                .HasDefaultValue(0);

            builder.HasMany(p => p.CartItems)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId);

            // Configure check constraints using ToTable
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Product_Price_NonNegative", "[Price] >= 0");
                t.HasCheckConstraint("CK_Product_OldPrice_GreaterThanPrice", "[OldPrice] IS NULL OR [OldPrice] > [Price]");
            });
        }
    }

}