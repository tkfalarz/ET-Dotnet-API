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

        public IQueryable<ReadingSet> GetDeviceReadings()
        {
            var distinctDatesWithDevices = dbContext.NumericReadings
                .Select(x => new { x.DeviceId, x.Timestamp })
                .Distinct();

            var aqi = from d in distinctDatesWithDevices
                join dv in dbContext.Devices on d.DeviceId equals dv.Id
                join a in dbContext.NumericReadings.Where(x => x.ReadingType == NumericReadingType.AirQualityIndex)
                    on new { d.DeviceId, d.Timestamp }
                    equals new { a.DeviceId, a.Timestamp }
                    into joinedAqi
                from j in joinedAqi.DefaultIfEmpty()
                select new ReadingSet
                {
                    DeviceName = dv.Name,
                    Timestamp = d.Timestamp,
                    AirQualityIndex = j.Value
                };

            var temperature = from d in distinctDatesWithDevices
                join dv in dbContext.Devices on d.DeviceId equals dv.Id
                join t in dbContext.NumericReadings.Where(x => x.ReadingType == NumericReadingType.Temperature)
                    on new { d.DeviceId, d.Timestamp }
                    equals new { t.DeviceId, t.Timestamp }
                    into joinedAqi
                from j in joinedAqi.DefaultIfEmpty()
                select new ReadingSet
                {
                    DeviceName = dv.Name,
                    Timestamp = d.Timestamp,
                    Temperature = j.Value
                };

            var humidity = from d in distinctDatesWithDevices
                join dv in dbContext.Devices on d.DeviceId equals dv.Id
                join h in dbContext.NumericReadings.Where(x => x.ReadingType == NumericReadingType.Humidity)
                    on new { d.DeviceId, d.Timestamp }
                    equals new { h.DeviceId, h.Timestamp }
                    into joined
                from j in joined.DefaultIfEmpty()
                select new ReadingSet
                {
                    DeviceName = dv.Name,
                    Timestamp = d.Timestamp,
                    Humidity = j.Value
                };

            var pressure = from d in distinctDatesWithDevices
                join dv in dbContext.Devices on d.DeviceId equals dv.Id
                join p in dbContext.NumericReadings.Where(x => x.ReadingType == NumericReadingType.Pressure)
                    on new { d.DeviceId, d.Timestamp }
                    equals new { p.DeviceId, p.Timestamp }
                    into joined
                from j in joined.DefaultIfEmpty()
                select new ReadingSet
                {
                    DeviceName = dv.Name,
                    Timestamp = d.Timestamp,
                    Pressure = j.Value
                };

            var combinedFactorsQuery = from a in aqi
                join t in temperature on new { a.Timestamp, a.DeviceName } equals new { t.Timestamp, t.DeviceName }
                join h in humidity on new { a.Timestamp, a.DeviceName } equals new { h.Timestamp, h.DeviceName }
                join p in pressure on new { a.Timestamp, a.DeviceName } equals new { p.Timestamp, p.DeviceName }
                select new ReadingSet
                {
                    Timestamp = a.Timestamp,
                    DeviceName = a.DeviceName,
                    AirQualityIndex = a.AirQualityIndex,
                    Temperature = t.Temperature,
                    Humidity = h.Humidity,
                    Pressure = p.Pressure
                };
            return combinedFactorsQuery;
        }

        public async Task StoreWeatherFactorsAsync(ReadingSet readingSet, Guid deviceId)
        {
            if (readingSet == null)
                throw new ArgumentNullException(nameof(readingSet),"Weather reading or device id cannot be null");

            if(readingSet.AirQualityIndex.HasValue)
                await dbContext.NumericReadings.AddAsync(
                    new NumericReading
                    {
                        DeviceId = deviceId,
                        Timestamp = readingSet.Timestamp,
                        ReadingType = NumericReadingType.AirQualityIndex,
                        Value = readingSet.AirQualityIndex.Value
                    });
            if(readingSet.Humidity.HasValue)
                await dbContext.NumericReadings.AddAsync(
                    new NumericReading
                    {
                        DeviceId = deviceId,
                        Timestamp = readingSet.Timestamp,
                        ReadingType = NumericReadingType.Humidity,
                        Value = readingSet.Humidity.Value
                    });
            if(readingSet.Pressure.HasValue)
                await dbContext.NumericReadings.AddAsync(
                    new NumericReading
                    {
                        DeviceId = deviceId,
                        Timestamp = readingSet.Timestamp,
                        ReadingType = NumericReadingType.Pressure,
                        Value = readingSet.Pressure.Value
                    });
            if(readingSet.Temperature.HasValue)
                await dbContext.NumericReadings.AddAsync(
                    new NumericReading
                    {
                        DeviceId = deviceId,
                        Timestamp = readingSet.Timestamp,
                        ReadingType = NumericReadingType.Temperature,
                        Value = readingSet.Temperature.Value
                    });

            await dbContext.SaveChangesAsync();
        }
    }
}