﻿using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.ModelsConfig
{
    public class ServiceConfig : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasColumnType("nvarchar(50)");
            builder.Property(x => x.Description).IsRequired().HasColumnType("nvarchar(400)");
            builder.Property(x => x.ImageUrl).IsRequired();
            builder.Property(ci => ci.RowVersion)
                .IsRowVersion()
                .HasConversion<byte[]>();
        }
    }
}