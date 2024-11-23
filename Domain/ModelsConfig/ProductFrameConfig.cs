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
    internal class ProductFrameConfig : IEntityTypeConfiguration<ProductFrame>
    {
        public void Configure(EntityTypeBuilder<ProductFrame> builder)
        {
            // Define composite primary key
            builder.HasKey(pf => new { pf.ProductId, pf.FrameId });

            // Configure relationships
            builder.HasOne(pf => pf.Product)
                .WithMany(p => p.ProductFrames)
                .HasForeignKey(pf => pf.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Ensures that when a Category is deleted, the associations are deleted


            builder.HasOne(pf => pf.Frame)
                .WithMany(f => f.ProductFrames)
                .HasForeignKey(pf => pf.FrameId)
                .OnDelete(DeleteBehavior.Cascade); // Ensures that when a Category is deleted, the associations are deleted

            // Optional: Add additional configurations or constraints if necessary (e.g., unique constraint)
            // If each product can only be associated with a category once, ensure the combination is unique
            builder.HasIndex(pf => new { pf.ProductId, pf.FrameId })
                .IsUnique();

        }
    }
}
