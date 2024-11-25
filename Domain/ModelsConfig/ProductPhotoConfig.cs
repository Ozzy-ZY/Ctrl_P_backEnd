using Domain.Models.ProductModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ModelsConfig
{
    internal class ProductPhotoConfig : IEntityTypeConfiguration<ProductPhoto>
    {
        public void Configure(EntityTypeBuilder<ProductPhoto> builder)
        {

            // Primary key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Url)
                .IsRequired() // URL is required
                .HasMaxLength(500); // Limit the length to 500 characters

            builder.Property(p => p.Description)
                .HasMaxLength(1000); // Optional, but limited to 1000 characters

            // Relationships
            builder.HasOne(p => p.Product) // A ProductPhoto belongs to one Product
                .WithMany(p => p.ProductPhotos) // A Product can have many ProductPhotos
                .HasForeignKey(p => p.ProductId) // Foreign key is ProductId
                .OnDelete(DeleteBehavior.Cascade); // Delete photos when the product is deleted
        }
    }
}
