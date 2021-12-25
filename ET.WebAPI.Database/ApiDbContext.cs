using ET.WebAPI.Database.Entities;
using ET.WebAPI.Database.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;

namespace ET.WebAPI.Database
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<NumericReading> NumericReadings { get; set; }
        public DbSet<Device> Devices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new AqiReadingsConfiguration())
                .ApplyConfiguration(new DevicesConfiguration());
        }

    }
}