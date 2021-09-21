using ET.WebAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ET.WebAPI.Api.Database
{
    public class PressureReadingsConfiguration : IEntityTypeConfiguration<PressureReading>
    {
        public void Configure(EntityTypeBuilder<PressureReading> builder)
        {
            builder
                .HasKey(x => new { x.Timestamp, x.Value })
                .IsClustered();
        }
    }
}