using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models;
using Domain.Models.ProductModels;

namespace Domain.ModelsConfig
{
    internal class ProductCategoryConfig : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            // Set composite primary key for the join table
            builder.HasKey(pc => new { pc.ProductId, pc.CategoryId });

            // Configure the relationship between Product and ProductCategory
            builder.HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Ensures that when a Product is deleted, its categories are deleted too

            // Configure the relationship between Category and ProductCategory
            builder.HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // Ensures that when a Category is deleted, the associations are deleted

            // Optional: Add additional configurations or constraints if necessary (e.g., unique constraint)
            // If each product can only be associated with a category once, ensure the combination is unique
            builder.HasIndex(pc => new { pc.ProductId, pc.CategoryId })
                .IsUnique();
        }
    }
}
