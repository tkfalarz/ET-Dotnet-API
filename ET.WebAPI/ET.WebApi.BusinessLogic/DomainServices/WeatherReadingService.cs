using ET.WebAPI.DatabaseAccess.DatabaseSetup;
using ET.WebAPI.DatabaseAccess.Entities;
using ET.WebAPI.Kernel;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.DomainServices;
using ET.WebAPI.Kernel.ErrorsHandling;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ET.WebAPI.BusinessLogic.DomainServices
{
    public class WeatherReadingService : IWeatherReadingService
    {
        private readonly ApiDbContext dbContext;

        public WeatherReadingService(ApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<OperationResult> StoreWeatherReadingAsync(WeatherReadingModel weatherReading)
        {
            if (weatherReading.Equals(default)) throw new ArgumentNullException(nameof(weatherReading));

            var device = await dbContext.Devices.FirstOrDefaultAsync(x => x.Name == weatherReading.DeviceName);
            if (device.Equals(default))
                return OperationResult.Failure("Device not exist", ErrorType.Internal);

            await dbContext.AqiReadings.AddAsync(
                new AqiReading
                {
                    DeviceId = device.Id,
                    Timestamp = weatherReading.Timestamp,
                    Value = weatherReading.AirQualityIndex
                });
            await dbContext.HumidityReadings.AddAsync(
                new HumidityReading
                {
                    DeviceId = device.Id,
                    Timestamp = weatherReading.Timestamp,
                    Value = weatherReading.Humidity
                });
            await dbContext.PressureReadings.AddAsync(
                new PressureReading
                {
                    DeviceId = device.Id,
                    Timestamp = weatherReading.Timestamp,
                    Value = weatherReading.Pressure
                });
            await dbContext.TemperatureReadings.AddAsync(
                new TemperatureReading
                {
                    DeviceId = device.Id,
                    Timestamp = weatherReading.Timestamp,
                    Value = weatherReading.Temperature
                });

            await dbContext.SaveChangesAsync();
            return OperationResult.Proceeded();
        }
    }
}