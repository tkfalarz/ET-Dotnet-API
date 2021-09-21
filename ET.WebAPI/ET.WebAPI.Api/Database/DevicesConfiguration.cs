using ET.WebAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ET.WebAPI.Api.Database
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
            builder
                .HasMany<AqiReading>()
                .WithOne(x => x.Device)
                .HasForeignKey(x => x.DeviceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasMany<HumidityReading>()
                .WithOne(x => x.Device)
                .HasForeignKey(x => x.DeviceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasMany<PressureReading>()
                .WithOne(x => x.Device)
                .HasForeignKey(x => x.DeviceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasMany<TemperatureReading>()
                .WithOne(x => x.Device)
                .HasForeignKey(x => x.DeviceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}