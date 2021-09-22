using ET.WebAPI.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ET.WebAPI.Api.DatabaseAccess.EntitiesConfiguration
{
    public class TemperatureReadingsConfiguration : IEntityTypeConfiguration<TemperatureReading>
    {
        public void Configure(EntityTypeBuilder<TemperatureReading> builder)
        {
            builder
                .HasKey(x => new { x.Timestamp, x.Value, x.DeviceId })
                .IsClustered();
        }
    }
}