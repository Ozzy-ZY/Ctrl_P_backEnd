using Domain.Models.ProductModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Domain.ModelsConfig
{
    internal class ProductReviewsConfig : IEntityTypeConfiguration<ProductReviews>
    {
        public void Configure(EntityTypeBuilder<ProductReviews> builder)
        {
            //Primary key
            builder.HasKey(pr => pr.Id);

            //Properties
            builder.Property(pr => pr.Rating)
                .IsRequired()
                .HasPrecision(2, 1)
                .HasDefaultValue(0);

            builder.Property(pr => pr.Review)
                .IsRequired() // Review is required
                .HasMaxLength(500); // Limit the length to 500 characters

            builder.Property(pr => pr.Name)
                   .IsRequired()
                   .HasMaxLength(100); // Adjust max length if needed

            builder.Property(pr => pr.Email)
                   .IsRequired()
                   .HasMaxLength(100); // Adjust max length if needed

            // Default Value for ReviewDate
            builder.Property(pr => pr.ReviewDate)
                   .HasDefaultValueSql("GETDATE()");

            // Relationships
            builder.HasOne(pr => pr.Product) // A ProductReview belongs to one Product
                .WithMany(p => p.ProductReviews) // A Product can have many ProductReviews
                .HasForeignKey(pr => pr.ProductId) // Foreign key is ProductId
                .OnDelete(DeleteBehavior.Cascade); // Delete reviews when the product is deleted

            builder.HasOne(pr => pr.Reviewer) // A ProductReview belongs to one AppUser
                .WithMany(u => u.Reviews) // A AppUser can have many ProductReviews
                .HasForeignKey(pr => pr.ReviewerId) // Foreign key is ReviewerId
                .OnDelete(DeleteBehavior.Cascade); // Delete reviews when the user is deleted

            // Configure check constraints for rating
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ProductReview_Rating_NonNegative", "[Rating] >= 0");
                t.HasCheckConstraint("CK_ProductReview_Rating_LessThanFive", "[Rating] <= 5");
            });
        }
    }
}