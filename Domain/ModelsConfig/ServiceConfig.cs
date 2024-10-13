using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}