using ET.WebAPI.DatabaseAccess.DatabaseSetup;
using ET.WebAPI.DatabaseAccess.Entities;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Device = ET.WebAPI.Kernel.DomainModels.Device;

namespace ET.WebAPI.DatabaseAccess.Repositories
{
    public class ReadingsRepository : IReadingsRepository
    {
        private readonly ApiDbContext dbContext;
        private readonly IDevicesRepository repository;

        public ReadingsRepository(ApiDbContext dbContext, IDevicesRepository repository)
        {
            this.dbContext = dbContext;
            this.repository = repository;
        }

        public async Task StoreWeatherFactorsAsync(WeatherReading weatherReading, Guid deviceId)
        {
            if (weatherReading == default || deviceId == default)
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