using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models.ProductModels;

namespace Domain.ModelsConfig
{
    internal class ProductSizeConfig : IEntityTypeConfiguration<ProductSize>
    {
        public void Configure(EntityTypeBuilder<ProductSize> builder)
        {
            // Define composite primary key
            builder.HasKey(ps => new { ps.ProductId, ps.SizeId });

            // Configure relationships
            builder.HasOne(ps => ps.Product)
                .WithMany(p => p.ProductSizes)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Ensures that when a Category is deleted, the associations are deleted

            builder.HasOne(ps => ps.Size)
                .WithMany(s => s.ProductSizes)
                .HasForeignKey(ps => ps.SizeId)
                .OnDelete(DeleteBehavior.Cascade); // Ensures that when a Category is deleted, the associations are deleted

            // Optional: Add additional configurations or constraints if necessary (e.g., unique constraint)
            // If each product can only be associated with a category once, ensure the combination is unique
            builder.HasIndex(ps => new { ps.ProductId, ps.SizeId })
                .IsUnique();
        }
    }
}
