using ET.WebAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ET.WebAPI.Api.Database
{
    public class TemperatureReadingsConfiguration : IEntityTypeConfiguration<TemperatureReading>
    {
        public void Configure(EntityTypeBuilder<TemperatureReading> builder)
        {
            builder
                .HasKey(x => new { x.Timestamp, x.Value })
                .IsClustered();
        }
    }
}