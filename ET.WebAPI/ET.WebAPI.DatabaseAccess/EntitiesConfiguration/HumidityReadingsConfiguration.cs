using ET.WebAPI.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ET.WebAPI.Api.DatabaseAccess.EntitiesConfiguration
{
    public class HumidityReadingsConfiguration : IEntityTypeConfiguration<HumidityReading>
    {
        public void Configure(EntityTypeBuilder<HumidityReading> builder)
        {
            builder
                .HasKey(x => new { x.Timestamp, x.Value, x.DeviceId })
                .IsClustered();
        }
    }
}