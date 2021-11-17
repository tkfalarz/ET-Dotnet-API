using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.DomainServices;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ET.WebAPI.BusinessLogic.Services
{
    public class ReadingsService : IReadingsService
    {
        private readonly IReadingsRepository readingsRepository;
        private readonly IDevicesRepository devicesRepository;

        public ReadingsService(
            IReadingsRepository readingsRepository,
            IDevicesRepository devicesRepository)
        {
            this.readingsRepository = readingsRepository;
            this.devicesRepository = devicesRepository;
        }

        public async Task<OperationResult> StoreWeatherReadingAsync(Reading reading)
        {
            if (reading == null) throw new ArgumentNullException(nameof(reading));

            var deviceId = await devicesRepository
                .GetDevices()
                .Where(x=>x.DeviceName == reading.DeviceName)
                .Select(x=>x.DeviceId).FirstOrDefaultAsync();
            if (deviceId == default)
                return OperationResult.Failure($"Device {reading.DeviceName} not found", ErrorType.BusinessLogic);

            var weatherReading = new Reading
            {
                Humidity = reading.Humidity,
                Pressure = reading.Pressure,
                Temperature = reading.Temperature,
                Timestamp = reading.Timestamp,
                AirQualityIndex = reading.AirQualityIndex
            };

            try
            {
                await readingsRepository.StoreWeatherFactorsAsync(weatherReading, deviceId);
            }
            catch (DbUpdateException exception)
            {
                return OperationResult.Failure(exception.Message, ErrorType.Entity);
            }
            
            return OperationResult.Proceeded();
        }

        public async Task<Reading> GetNearestLatestReadingAsync(decimal latitude, decimal longitude)
        {
            var devices = await devicesRepository.GetDevices().ToArrayAsync();
            var devicesDistances = new SortedSet<(double distanceFromUserLocation, string deviceName)>();
            var result = (Reading)default;
            
            foreach (var device in devices)
            {
                var coordsDistance = GetCoordsDistance(latitude, longitude, device.Latitude, device.Longitude);
                devicesDistances.Add((coordsDistance, device.DeviceName));
            }

            if (devicesDistances.Any())
            {
                var queryable = await readingsRepository
                    .GetDeviceReadings()
                    .OrderByDescending(x=>x.Timestamp)
                    .ToArrayAsync();
                
                foreach (var (distance, deviceName) in devicesDistances)
                {
                    var filtered = queryable.Where(x=>x.DeviceName==deviceName).Take(1);
                    if (filtered.Any())
                    {
                        result = filtered.First();
                        break;
                    }
                }
            }

            return result;
        }

        public async Task<List<Reading>> GetLatestReadingsAsync()
        {
            var devicesNames = await devicesRepository.GetDevices().Select(x=>x.DeviceName).ToArrayAsync();
            var readings = new List<Reading>();

            if (devicesNames.Any())
            {
                var orderedSensorReadingsQuery =  await readingsRepository
                    .GetDeviceReadings()
                    .OrderByDescending(x => x.Timestamp)
                    .ToArrayAsync();
                
                foreach (var deviceName in devicesNames)
                {
                    var res = orderedSensorReadingsQuery
                        .Where(x => x.DeviceName == deviceName)
                        .Take(1)
                        .ToArray();
                
                    readings.AddRange(res);
                }
            }

            return readings;
        }

        public async Task<Reading[]> GetDeviceReadingsAsync(string deviceName, int limit)
        {
            var readings = await readingsRepository.GetDeviceReadings().ToArrayAsync();
            var filteredByDeviceNameQuery = readings.Where(x => x.DeviceName == deviceName);
            return limit == 0
                ? filteredByDeviceNameQuery.ToArray()
                : filteredByDeviceNameQuery.OrderByDescending(x => x.Timestamp).Take(limit).ToArray();
        }

        private double GetCoordsDistance(decimal latitude, decimal longitude, decimal deviceLatitude, decimal deviceLongitude)
        {
            var xDiff = Math.Pow((double)latitude - (double)deviceLatitude, 2);
            var yDiff = Math.Pow((double)longitude - (double)deviceLongitude, 2);

            var result = Math.Abs(Math.Sqrt(xDiff + yDiff));
            return result;
        }
    }
}