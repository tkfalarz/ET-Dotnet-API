using ET.WebAPI.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ET.WebAPI.Api.DatabaseAccess.EntitiesConfiguration
{
    public class DevicesConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(p => p.Name).HasMaxLength(50);
            builder.Property(p => p.Latitude).IsRequired();
            builder.Property(p => p.Longitude).IsRequired();
            builder.Property(x => x.SensorName).IsRequired();
        }
    }
}