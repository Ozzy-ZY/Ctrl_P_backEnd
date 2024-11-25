using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models.CategorizingModels; // Assuming Material resides in this namespace

namespace Domain.ModelsConfig
{
    internal class MaterialConfig : IEntityTypeConfiguration<Material>
    {
        public void Configure(EntityTypeBuilder<Material> builder)
        {

            // Optional: Configure table name if it differs from the class name
            builder.ToTable("Materials");

            // Define the primary key
            builder.HasKey(m => m.Id);

            // Configure properties
            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100); // Example: Name cannot exceed 100 characters

            // Optional: Configure a unique constraint if the Name should be unique
            builder.HasIndex(c => c.Name)
                .IsUnique();
        }
    }
}
