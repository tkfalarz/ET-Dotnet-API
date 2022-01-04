using ET.WebAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ET.WebAPI.Database.EntitiesConfiguration
{
    internal class AqiReadingsConfiguration : IEntityTypeConfiguration<NumericReading>
    {
        public void Configure(EntityTypeBuilder<NumericReading> builder)
        {
            builder
                .HasKey(x => new { x.ReadingType, x.Timestamp, x.Value, x.DeviceId })
                .IsClustered();
        }
    }
}