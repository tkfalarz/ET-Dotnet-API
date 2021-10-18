using ET.WebAPI.DatabaseAccess.DatabaseSetup;
using ET.WebAPI.DatabaseAccess.Entities;
using ET.WebAPI.Kernel;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.DomainServices;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Device = ET.WebAPI.Kernel.DomainModels.Device;

namespace ET.WebAPI.BusinessLogic.DomainServices
{
    public class ReadingsService : IReadingsService
    {
        private readonly ApiDbContext dbContext;
        private readonly IReadingsRepository readingsRepository;
        private readonly IDevicesRepository devicesRepository;

        public ReadingsService(
            ApiDbContext dbContext,
            IReadingsRepository readingsRepository,
            IDevicesRepository devicesRepository)
        {
            this.dbContext = dbContext;
            this.readingsRepository = readingsRepository;
            this.devicesRepository = devicesRepository;
        }

        public async Task<OperationResult> StoreWeatherReadingAsync(DeviceReading deviceReading)
        {
            if (deviceReading.Equals(default)) throw new ArgumentNullException(nameof(deviceReading));

            var deviceIdGetOperationResult = await devicesRepository.GetDeviceIdAsync(deviceReading.DeviceName);
            if (deviceIdGetOperationResult.IsFailure)
                return OperationResult.Failure(deviceIdGetOperationResult.ErrorMessage, deviceIdGetOperationResult.ErrorType);

            var weatherReading = new WeatherReading
            {
                Humidity = deviceReading.Humidity,
                Pressure = deviceReading.Pressure,
                Temperature = deviceReading.Temperature,
                Timestamp = deviceReading.Timestamp,
                AirQualityIndex = deviceReading.AirQualityIndex
            };

            try
            {
                await readingsRepository.StoreWeatherFactorsAsync(weatherReading, deviceIdGetOperationResult.Value);
            }
            catch (DbUpdateException exception)
            {
                return OperationResult.Failure(exception.Message, ErrorType.Entity);
            }
            
            return OperationResult.Proceeded();
        }
    }
}