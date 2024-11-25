using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models.CategorizingModels; // Assuming Size resides in this namespace

namespace Domain.ModelsConfig
{
    internal class SizeConfig : IEntityTypeConfiguration<Size>
    {
        public void Configure(EntityTypeBuilder<Size> builder)
        {
            // Optional: Configure table name if it differs from the class name
            builder.ToTable("Sizes");

            // Define the primary key
            builder.HasKey(s => s.Id);

            // Configure properties
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50); // Example: Name cannot exceed 50 characters

            // Optional: Configure a unique constraint if the Name should be unique
            builder.HasIndex(c => c.Name)
                .IsUnique();
        }
    }
}
