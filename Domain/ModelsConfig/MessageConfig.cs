using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.ModelsConfig;

public class MessageConfig:IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.Property(m => m.ContactEmail).IsRequired();
        builder.Property(m=> m.Content).IsRequired().HasMaxLength(500);
        builder.Property(m=> m.Subject).IsRequired().HasMaxLength(100);
        builder.HasOne(m => m.User).WithMany(u => u.Messages);
    }
}