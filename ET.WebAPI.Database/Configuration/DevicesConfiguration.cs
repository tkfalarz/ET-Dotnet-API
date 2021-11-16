using ET.WebAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ET.WebAPI.Database.EntitiesConfiguration
{
    internal class DevicesConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(p => p.Name).HasMaxLength(50);
            builder.Property(p => p.Latitude).IsRequired().HasPrecision(8, 5);
            builder.Property(p => p.Longitude).IsRequired().HasPrecision(9, 5);
            builder.Property(x => x.SensorName).IsRequired();
        }
    }
}