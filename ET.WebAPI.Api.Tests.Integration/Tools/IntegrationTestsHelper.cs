using ET.WebAPI.Database;
using ET.WebAPI.Database.Entities;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace ET.WebAPI.Api.Tests.Integration.Tools
{
    public static class IntegrationTestsHelper
    {
        public const string Device1Name = "Device1";
        public const string SensorName = "Sensor";
        public const decimal Coordinate = 11.1111m;
        public const double ReadingValue = 12;
        public static readonly Guid Device1Id = new("0e72a931-8473-47d0-a7cf-ba6b67e8de50");
        public static readonly DateTimeOffset ReadingTimestamp1 = DateTimeOffset.Now;
        public static readonly DateTimeOffset ReadingTimestamp2 = DateTimeOffset.Now.AddHours(-1);

        public static T ConvertToObject<T>(HttpContent result) where T : class
            => JsonConvert.DeserializeObject<T>(result.ReadAsStringAsync().GetAwaiter().GetResult());
        
        public static void AddMinimalDatabaseData(ApiDbContext dbContext)
        {
            dbContext.Devices.Add(new Device { Id = Device1Id, Name = Device1Name, Latitude = Coordinate, Longitude = Coordinate, SensorName = SensorName });
            dbContext.SaveChanges();
            dbContext.NumericReadings.Add(new NumericReading { ReadingType = NumericReadingType.AirQualityIndex, DeviceId = Device1Id, Timestamp = ReadingTimestamp1, Value = ReadingValue });
            dbContext.NumericReadings.Add(new NumericReading { ReadingType = NumericReadingType.Humidity, DeviceId = Device1Id, Timestamp = ReadingTimestamp1, Value = ReadingValue });
            dbContext.NumericReadings.Add(new NumericReading { ReadingType = NumericReadingType.Temperature, DeviceId = Device1Id, Timestamp = ReadingTimestamp1, Value = ReadingValue });
            dbContext.NumericReadings.Add(new NumericReading { ReadingType = NumericReadingType.Pressure, DeviceId = Device1Id, Timestamp = ReadingTimestamp1, Value = ReadingValue });
            dbContext.NumericReadings.Add(new NumericReading { ReadingType = NumericReadingType.AirQualityIndex, DeviceId = Device1Id, Timestamp = ReadingTimestamp2, Value = ReadingValue });
            dbContext.NumericReadings.Add(new NumericReading { ReadingType = NumericReadingType.Humidity, DeviceId = Device1Id, Timestamp = ReadingTimestamp2, Value = ReadingValue });
            dbContext.NumericReadings.Add(new NumericReading { ReadingType = NumericReadingType.Temperature, DeviceId = Device1Id, Timestamp = ReadingTimestamp2, Value = ReadingValue });
            dbContext.NumericReadings.Add(new NumericReading { ReadingType = NumericReadingType.Pressure, DeviceId = Device1Id, Timestamp = ReadingTimestamp2, Value = ReadingValue });
            dbContext.SaveChanges();
        }
    }
}