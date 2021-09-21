using ET.WebAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ET.WebAPI.Api.Database
{
    public class HumidityReadingsConfiguration : IEntityTypeConfiguration<HumidityReading>
    {
        public void Configure(EntityTypeBuilder<HumidityReading> builder)
        {
            builder
                .HasKey(x => new { x.Timestamp, x.Value })
                .IsClustered();
        }
    }
}