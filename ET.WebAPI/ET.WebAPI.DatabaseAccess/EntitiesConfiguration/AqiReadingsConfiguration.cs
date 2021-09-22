using ET.WebAPI.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ET.WebAPI.Api.DatabaseAccess.EntitiesConfiguration
{
    public class AqiReadingsConfiguration : IEntityTypeConfiguration<AqiReading>
    {
        public void Configure(EntityTypeBuilder<AqiReading> builder)
        {
            builder
                .HasKey(x => new { x.Timestamp, x.Value, x.DeviceId })
                .IsClustered();
        }
    }
}