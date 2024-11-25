using Domain.Models.CategorizingModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ModelsConfig
{
    internal class FrameConfig : IEntityTypeConfiguration<Frame>
    {
        public void Configure(EntityTypeBuilder<Frame> builder)
        {
            // Optional: Configure table name if it differs from the class name
            builder.ToTable("Frames");

            // Define the primary key
            builder.HasKey(f => f.Id);

            // Configure properties
            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100); // Example: Name cannot exceed 100 characters

            // Optional: Configure a unique constraint if the Name should be unique
            builder.HasIndex(c => c.Name)
                .IsUnique();
        }
    }
}
