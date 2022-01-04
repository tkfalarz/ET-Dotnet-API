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

        public async Task<OperationResult> StoreReadingSetAsync(ReadingSet readingSet)
        {
            if (readingSet == null) throw new ArgumentNullException(nameof(readingSet));

            var deviceId = await devicesRepository
                .GetDevices()
                .Where(x=>x.DeviceName == readingSet.DeviceName)
                .Select(x=>x.DeviceId).FirstOrDefaultAsync();
            if (deviceId == default)
                return OperationResult.Failure($"Device {readingSet.DeviceName} not found", ErrorType.BusinessLogic);

            var weatherReading = new ReadingSet
            {
                Humidity = readingSet.Humidity,
                Pressure = readingSet.Pressure,
                Temperature = readingSet.Temperature,
                Timestamp = readingSet.Timestamp,
                AirQualityIndex = readingSet.AirQualityIndex
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

        public async Task<ReadingSet> GetLatestReadingsAsync(string deviceName)
        {
            var devices = readingsRepository
                .GetDeviceReadings()
                .OrderByDescending(x => x.Timestamp)
                .Where(x => x.DeviceName == deviceName)
                .FirstOrDefaultAsync();

            return await devices;
        }

        public async Task<ReadingSet> GetNearestLatestReadingsAsync(decimal latitude, decimal longitude)
        {
            var devices = await devicesRepository.GetDevices().ToArrayAsync();
            var devicesDistances = new SortedSet<(double distanceFromUserLocation, string deviceName)>();
            var result = (ReadingSet)default;
            
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
                
                foreach (var (_, deviceName) in devicesDistances)
                {
                    var filtered = queryable
                        .Where(x => x.DeviceName == deviceName)
                        .Take(1)
                        .ToList();

                    if (!filtered.Any()) continue;

                    result = filtered.First();
                    break;
                }
            }

            return result;
        }

        public async Task<IReadOnlyList<ReadingSet>> GetReadingsAsync(string deviceName, int limit)
        {
            var readings = readingsRepository
                .GetDeviceReadings()
                .Where(x => x.DeviceName == deviceName)
                .OrderByDescending(x=>x.Timestamp);

            return limit > 0
                ? await readings.Take(limit).ToListAsync()
                : await readings.ToListAsync();
        }

        public Task<Reading> GetTypedLatestReadingAsync(string deviceName, ReadingType readingType)
        {
            var result = readingsRepository
                .GetDeviceReadings()
                .Where(x => x.DeviceName == deviceName);

            var filtered = NarrowQueryByReadingType(readingType, result);

            return filtered.OrderByDescending(x => x.Timestamp).Take(1).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Reading>> GetTypedReadingsAsync(string deviceName, ReadingType readingType, int limit)
        {
            var query = readingsRepository
                .GetDeviceReadings()
                .Where(x => x.DeviceName == deviceName);

            var filtered = NarrowQueryByReadingType(readingType, query).OrderByDescending(x=>x.Timestamp);

            return limit > 0
                ? await filtered.Take(limit).ToListAsync()
                : await filtered.ToListAsync();
        }

        private static IQueryable<Reading> NarrowQueryByReadingType(ReadingType readingType, IQueryable<ReadingSet> queryable)
        {
            var filtered = readingType switch
            {
                ReadingType.Temperature => queryable.Where(x=>x.Temperature.HasValue).Select(x => new Reading { Timestamp = x.Timestamp, Value = x.Temperature.Value }),
                ReadingType.Humidity => queryable.Where(x=>x.Humidity.HasValue).Select(x => new Reading { Timestamp = x.Timestamp, Value = x.Humidity.Value }),
                ReadingType.Pressure => queryable.Where(x=>x.Pressure.HasValue).Select(x => new Reading { Timestamp = x.Timestamp, Value = x.Pressure.Value }),
                ReadingType.Aqi => queryable.Where(x=>x.AirQualityIndex.HasValue).Select(x => new Reading { Timestamp = x.Timestamp, Value = x.AirQualityIndex.Value }),
                _ => throw new InvalidOperationException("Not supported reading type")
            };
            return filtered;
        }
        
        private static double GetCoordsDistance(decimal latitude, decimal longitude, decimal deviceLatitude, decimal deviceLongitude)
        {
            var xDiff = Math.Pow((double)latitude - (double)deviceLatitude, 2);
            var yDiff = Math.Pow((double)longitude - (double)deviceLongitude, 2);

            var result = Math.Abs(Math.Sqrt(xDiff + yDiff));
            return result;
        }
    }
}