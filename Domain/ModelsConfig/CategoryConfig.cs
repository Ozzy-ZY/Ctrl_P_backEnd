using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models;
using Domain.Models.CategorizingModels;

namespace Domain.ModelsConfig
{
    internal class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Set table name if it's different from the default (optional)
            builder.ToTable("Categories");

            // Set primary key (optional if default conventions work)
            builder.HasKey(c => c.Id);

            // Set property configurations
            builder.Property(c => c.Name)
                .IsRequired()  // Ensures the Name is required
                .HasMaxLength(100); // Sets maximum length for Name
            builder.Property(c => c.ImageUrl)
                .IsRequired() // URL is required
                .HasMaxLength(500); // Limit the length to 500 characters
            // Optional: Configure a unique constraint if the Name should be unique
            builder.HasIndex(c => c.Name)
                .IsUnique();
        }
    }
}
