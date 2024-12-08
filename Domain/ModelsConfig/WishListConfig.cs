using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models.ShopModels;

namespace Domain.ModelsConfig;

public class WishListConfig : IEntityTypeConfiguration<WishList>
{
    public void Configure(EntityTypeBuilder<WishList> builder)
    {
        builder.HasKey(w => new { w.UserId, w.ProductId });

        builder.HasOne(w => w.AppUser)
            .WithMany(u => u.WishLists)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.Product)
            .WithMany(p => p.WishLists)
            .HasForeignKey(w => w.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(w => w.AddedAt)
                   .HasDefaultValueSql("GETDATE()");

        builder.HasIndex(w => new { w.UserId, w.ProductId })
            .IsUnique();
    }
}
