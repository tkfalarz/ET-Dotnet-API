using ET.WebAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ET.WebAPI.Api.Database
{
    public class AqiReadingsConfiguration : IEntityTypeConfiguration<AqiReading>
    {
        public void Configure(EntityTypeBuilder<AqiReading> builder)
        {
            builder
                .HasKey(x => new { x.Timestamp, x.Value })
                .IsClustered();
        }
    }
}