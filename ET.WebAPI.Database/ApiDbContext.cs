using ET.WebAPI.Database.Entities;
using ET.WebAPI.Database.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;

namespace ET.WebAPI.Database
{
    public class ApiDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ApiDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AqiReading> AqiReadings { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<HumidityReading> HumidityReadings { get; set; }
        public DbSet<PressureReading> PressureReadings { get; set; }
        public DbSet<TemperatureReading> TemperatureReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new AqiReadingsConfiguration())
                .ApplyConfiguration(new DevicesConfiguration())
                .ApplyConfiguration(new HumidityReadingsConfiguration())
                .ApplyConfiguration(new PressureReadingsConfiguration())
                .ApplyConfiguration(new TemperatureReadingsConfiguration());
        }

    }
}