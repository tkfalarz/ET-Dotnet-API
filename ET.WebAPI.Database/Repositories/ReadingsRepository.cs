using ET.WebAPI.Database.Entities;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Device = ET.WebAPI.Kernel.DomainModels.Device;

namespace ET.WebAPI.Database.Repositories
{
    public class ReadingsRepository : IReadingsRepository
    {
        private readonly ApiDbContext dbContext;

        public ReadingsRepository(ApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task StoreWeatherFactorsAsync(WeatherReading weatherReading, Guid deviceId)
        {
            if (weatherReading == null)
                throw new ArgumentNullException("Weather reading or device id cannot be null");

            await dbContext.AqiReadings.AddAsync(
                new AqiReading
                {
                    DeviceId = deviceId,
                    Timestamp = weatherReading.Timestamp,
                    Value = weatherReading.AirQualityIndex
                });
            await dbContext.HumidityReadings.AddAsync(
                new HumidityReading
                {
                    DeviceId = deviceId,
                    Timestamp = weatherReading.Timestamp,
                    Value = weatherReading.Humidity
                });
            await dbContext.PressureReadings.AddAsync(
                new PressureReading
                {
                    DeviceId = deviceId,
                    Timestamp = weatherReading.Timestamp,
                    Value = weatherReading.Pressure
                });
            await dbContext.TemperatureReadings.AddAsync(
                new TemperatureReading
                {
                    DeviceId = deviceId,
                    Timestamp = weatherReading.Timestamp,
                    Value = weatherReading.Temperature
                });

            await dbContext.SaveChangesAsync();
        }
    }
}