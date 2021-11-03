using ET.WebAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ET.WebAPI.Database.EntitiesConfiguration
{
    internal class PressureReadingsConfiguration : IEntityTypeConfiguration<PressureReading>
    {
        public void Configure(EntityTypeBuilder<PressureReading> builder)
        {
            builder
                .HasKey(x => new { x.Timestamp, x.Value, x.DeviceId })
                .IsClustered();
        }
    }
}