using ET.WebAPI.Database;
using ET.WebAPI.Database.Entities;
using ET.WebAPI.Database.Repositories;
using ET.WebApi.Database.Tests.Integration.Utilities;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.Repositories;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Device = ET.WebAPI.Database.Entities.Device;

namespace ET.WebApi.Database.Tests.Integration.Repositories
{
    [TestFixture]
    public class ReadingsRepositoryTests
    {
        private const int AirQualityIndexValue = 98;
        private const int HumidityValue = 22;
        private const int TemperatureValue = 18;
        private const int PressureValue = 13;
        private static readonly Guid Device1Id = new("7353cef6-5456-4c9e-b9ba-a5cf9b141004");
        private static readonly Guid Device2Id = new("fb6ac3b1-624a-4523-b899-0acb776ee5b4");
        private const string Device1Name = "Happy device";
        private const string Device2Name = "Happy device #2";
        private static readonly DateTimeOffset Timestamp = DateTimeOffset.Now;
        private IReadingsRepository repository;
        private ApiDbContext dbContext;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            dbContext = TestDatabaseCreator.CreateTestDatabaseContext();
            dbContext.Devices.AddRange(
                new Device
                {
                    Id = Device1Id,
                    Latitude = 11.2124m,
                    Longitude = 18.1241m,
                    Name = Device1Name,
                    SensorName = "Happy sensor"
                },
                new Device
                {
                    Id = Device2Id,
                    Latitude = 21.2124m,
                    Longitude = 28.1241m,
                    Name = Device2Name,
                    SensorName = "Happy sensor #2"
                });
            dbContext.SaveChanges();
        }

        [SetUp]
        public void BeforeEach()
        {
            repository = new ReadingsRepository(dbContext);
        }

        [TearDown]
        public void AfterEach()
        {
            dbContext.NumericReadings.RemoveRange(dbContext.NumericReadings);
            dbContext.SaveChanges();
        }

        [Test]
        public async Task StoreWeatherFactorsAsyncStoresWeatherFactors()
        {
            var reading = new ReadingSet
            {
                Humidity = HumidityValue,
                Pressure = PressureValue,
                Temperature = TemperatureValue,
                AirQualityIndex = AirQualityIndexValue,
                Timestamp = Timestamp
            };
            var expectedResult = new NumericReading[]
            {
                new()
                {
                    DeviceId = Device1Id,
                    Timestamp = Timestamp,
                    ReadingType = NumericReadingType.AirQualityIndex,
                    Value = AirQualityIndexValue
                },
                new()
                {
                    DeviceId = Device1Id,
                    Timestamp = Timestamp,
                    ReadingType = NumericReadingType.Humidity,
                    Value = HumidityValue
                },
                new()
                {
                    DeviceId = Device1Id,
                    Timestamp = Timestamp,
                    ReadingType = NumericReadingType.Temperature,
                    Value = TemperatureValue
                },
                new()
                {
                    DeviceId = Device1Id,
                    Timestamp = Timestamp,
                    ReadingType = NumericReadingType.Pressure,
                    Value = PressureValue
                }
            };

            await repository.StoreWeatherFactorsAsync(reading, Device1Id);

            dbContext.NumericReadings
                .Where(x => x.DeviceId == Device1Id).Should()
                .BeEquivalentTo(expectedResult, opt => opt.Excluding(x => x.Device));
        }

        [Test]
        public void GetDeviceReadingsTest()
        { 
            var date1 = new DateTimeOffset(DateTime.Now);
            var date2 = new DateTimeOffset(DateTime.Now.AddMinutes(30));
            AddDevicesReadings(date1, date2);
            var expectedResult = new[]
            {
                new ReadingSet
                {
                    Timestamp = date1,
                    Humidity = HumidityValue,
                    Pressure = PressureValue,
                    Temperature = TemperatureValue,
                    AirQualityIndex = AirQualityIndexValue,
                    DeviceName = Device1Name
                },
                new ReadingSet
                {
                    Timestamp = date2,
                    Humidity = HumidityValue,
                    Pressure = PressureValue,
                    Temperature = TemperatureValue,
                    AirQualityIndex = AirQualityIndexValue,
                    DeviceName = Device1Name
                },
                new ReadingSet
                {
                    Timestamp = date1,
                    Humidity = HumidityValue,
                    Pressure = PressureValue,
                    Temperature = TemperatureValue,
                    AirQualityIndex = AirQualityIndexValue,
                    DeviceName = Device2Name
                }
            };

            var result = repository.GetDeviceReadings();

            result.Should().BeEquivalentTo(expectedResult.AsQueryable());
        }

        [Test]
        public void GetDeviceReadingsPartialTest()
        {
            var expectedResult = new[]
            {
                new ReadingSet
                {
                    Timestamp = Timestamp,
                    DeviceName = Device1Name,
                    Humidity = HumidityValue,
                    Temperature = TemperatureValue,
                    Pressure = null,
                    AirQualityIndex = null
                }
            };
            dbContext.NumericReadings.AddRange(
                new NumericReading
                {
                    DeviceId = Device1Id,
                    ReadingType = NumericReadingType.Temperature,
                    Timestamp = Timestamp,
                    Value = TemperatureValue
                },
                new NumericReading
                {
                    DeviceId = Device1Id,
                    ReadingType = NumericReadingType.Humidity,
                    Timestamp = Timestamp,
                    Value = HumidityValue
                });
            dbContext.SaveChanges();

            var result = repository.GetDeviceReadings();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void GetDeviceReadingsEmptyCase()
        {
            var result = repository.GetDeviceReadings();

            result.Should().BeEmpty();
        }
        
        private void AddDevicesReadings(DateTimeOffset date1, DateTimeOffset date2)
        {
            dbContext.NumericReadings.AddRange(
                new NumericReading { Timestamp = date1, Value = AirQualityIndexValue, ReadingType = NumericReadingType.AirQualityIndex, DeviceId = Device1Id },
                new NumericReading { Timestamp = date2, Value = AirQualityIndexValue, ReadingType = NumericReadingType.AirQualityIndex, DeviceId = Device1Id },
                new NumericReading { Timestamp = date1, Value = AirQualityIndexValue, ReadingType = NumericReadingType.AirQualityIndex, DeviceId = Device2Id });
            dbContext.NumericReadings.AddRange(
                new NumericReading { Timestamp = date1, Value = PressureValue, ReadingType = NumericReadingType.Pressure, DeviceId = Device1Id },
                new NumericReading { Timestamp = date2, Value = PressureValue, ReadingType = NumericReadingType.Pressure, DeviceId = Device1Id },
                new NumericReading { Timestamp = date1, Value = PressureValue, ReadingType = NumericReadingType.Pressure, DeviceId = Device2Id });
            dbContext.NumericReadings.AddRange(
                new NumericReading { Timestamp = date1, Value = TemperatureValue, ReadingType = NumericReadingType.Temperature, DeviceId = Device1Id },
                new NumericReading { Timestamp = date2, Value = TemperatureValue, ReadingType = NumericReadingType.Temperature, DeviceId = Device1Id },
                new NumericReading { Timestamp = date1, Value = TemperatureValue, ReadingType = NumericReadingType.Temperature, DeviceId = Device2Id });
            dbContext.NumericReadings.AddRange(
                new NumericReading { Timestamp = date1, Value = HumidityValue, ReadingType = NumericReadingType.Humidity,  DeviceId = Device1Id },
                new NumericReading { Timestamp = date2, Value = HumidityValue, ReadingType = NumericReadingType.Humidity, DeviceId = Device1Id },
                new NumericReading { Timestamp = date1, Value = HumidityValue, ReadingType = NumericReadingType.Humidity, DeviceId = Device2Id });
            dbContext.SaveChanges();
        }
    }
}