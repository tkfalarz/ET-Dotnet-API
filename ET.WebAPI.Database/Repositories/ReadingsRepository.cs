using ET.WebAPI.Database.Entities;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ET.WebAPI.Database.Repositories
{
    public class ReadingsRepository : IReadingsRepository
    {
        private readonly ApiDbContext dbContext;

        public ReadingsRepository(ApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task StoreWeatherFactorsAsync(Reading reading, Guid deviceId)
        {
            if (reading == null)
                throw new ArgumentNullException("Weather reading or device id cannot be null");

            await dbContext.AqiReadings.AddAsync(
                new AqiReading
                {
                    DeviceId = deviceId,
                    Timestamp = reading.Timestamp,
                    Value = reading.AirQualityIndex
                });
            await dbContext.HumidityReadings.AddAsync(
                new HumidityReading
                {
                    DeviceId = deviceId,
                    Timestamp = reading.Timestamp,
                    Value = reading.Humidity
                });
            await dbContext.PressureReadings.AddAsync(
                new PressureReading
                {
                    DeviceId = deviceId,
                    Timestamp = reading.Timestamp,
                    Value = reading.Pressure
                });
            await dbContext.TemperatureReadings.AddAsync(
                new TemperatureReading
                {
                    DeviceId = deviceId,
                    Timestamp = reading.Timestamp,
                    Value = reading.Temperature
                });

            await dbContext.SaveChangesAsync();
        }

        public async Task<IQueryable<Reading>> GetDeviceReadingsAsync()
        {
            var query =
                from a in dbContext.AqiReadings
                join h in dbContext.HumidityReadings on new { a.DeviceId, a.Timestamp } equals new { h.DeviceId, h.Timestamp }
                join p in dbContext.PressureReadings on new { a.DeviceId, a.Timestamp } equals new { p.DeviceId, p.Timestamp }
                join t in dbContext.TemperatureReadings on new { a.DeviceId, a.Timestamp } equals new { t.DeviceId, t.Timestamp }
                join d in dbContext.Devices on a.DeviceId equals d.Id
                select new Reading
                {
                    Timestamp = a.Timestamp,
                    AirQualityIndex = a.Value,
                    Humidity = h.Value,
                    Pressure = p.Value,
                    Temperature = t.Value,
                    DeviceName = d.Name
                };
            
            return query;
        }
    }
}