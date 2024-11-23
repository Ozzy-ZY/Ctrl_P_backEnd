using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models.ProductModels;

namespace Domain.ModelsConfig
{
    internal class ProductMaterialConfig : IEntityTypeConfiguration<ProductMaterial>
    {
        public void Configure(EntityTypeBuilder<ProductMaterial> builder)
        {
            // Define composite primary key
            builder.HasKey(pm => new { pm.ProductId, pm.MaterialId });

            // Configure relationships
            builder.HasOne(pm => pm.Product)
                .WithMany(p => p.ProductMaterials)
                .HasForeignKey(pm => pm.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Ensures that when a Product is deleted, its categories are deleted too


            builder.HasOne(pm => pm.Material)
                .WithMany(m => m.ProductMaterials)
                .HasForeignKey(pm => pm.MaterialId)
                .OnDelete(DeleteBehavior.Cascade); // Ensures that when a Product is deleted, its categories are deleted too

            // Optional: Add additional configurations or constraints if necessary (e.g., unique constraint)
            // If each product can only be associated with a category once, ensure the combination is unique
            builder.HasIndex(pm => new { pm.ProductId, pm.MaterialId })
                .IsUnique();

        }
    }
}
