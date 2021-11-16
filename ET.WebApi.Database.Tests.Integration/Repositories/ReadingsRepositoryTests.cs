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
            dbContext.AqiReadings.RemoveRange(dbContext.AqiReadings);
            dbContext.HumidityReadings.RemoveRange(dbContext.HumidityReadings);
            dbContext.PressureReadings.RemoveRange(dbContext.PressureReadings);
            dbContext.TemperatureReadings.RemoveRange(dbContext.TemperatureReadings);
            dbContext.SaveChanges();
        }

        [Test]
        public async Task StoreWeatherFactorsAsyncStoresWeatherFactors()
        {
            var reading = new Reading
            {
                Humidity = HumidityValue,
                Pressure = PressureValue,
                Temperature = TemperatureValue,
                AirQualityIndex = AirQualityIndexValue,
                Timestamp = Timestamp
            };

            await repository.StoreWeatherFactorsAsync(reading, Device1Id);

            dbContext.AqiReadings.First(x => x.DeviceId == Device1Id).Should().BeEquivalentTo(
                new AqiReading
                {
                    DeviceId = Device1Id,
                    Timestamp = Timestamp,
                    Value = AirQualityIndexValue
                },
                opt => opt.Excluding(x => x.Device));
            dbContext.HumidityReadings.First(x => x.DeviceId == Device1Id).Should().BeEquivalentTo(
                new HumidityReading
                {
                    DeviceId = Device1Id,
                    Timestamp = Timestamp,
                    Value = HumidityValue
                },
                opt => opt.Excluding(x => x.Device));
            dbContext.TemperatureReadings.First(x => x.DeviceId == Device1Id).Should().BeEquivalentTo(
                new TemperatureReading
                {
                    DeviceId = Device1Id,
                    Timestamp = Timestamp,
                    Value = TemperatureValue
                },
                opt => opt.Excluding(x => x.Device));
            dbContext.PressureReadings.First(x => x.DeviceId == Device1Id).Should().BeEquivalentTo(
                new PressureReading
                {
                    DeviceId = Device1Id,
                    Timestamp = Timestamp,
                    Value = PressureValue
                },
                opt => opt.Excluding(x => x.Device));
        }

        [Test]
        public async Task GetDeviceReadingsAsyncTest()
        { 
            var date1 = new DateTimeOffset(DateTime.Now);
            var date2 = new DateTimeOffset(DateTime.Now.AddMinutes(30));
            AddDevicesReadings(date1, date2);
            var expectedResult = new[]
            {
                new Reading
                {
                    Timestamp = date1,
                    Humidity = HumidityValue,
                    Pressure = PressureValue,
                    Temperature = TemperatureValue,
                    AirQualityIndex = AirQualityIndexValue,
                    DeviceName = Device1Name
                },
                new Reading
                {
                    Timestamp = date2,
                    Humidity = HumidityValue,
                    Pressure = PressureValue,
                    Temperature = TemperatureValue,
                    AirQualityIndex = AirQualityIndexValue,
                    DeviceName = Device1Name
                },
                new Reading
                {
                    Timestamp = date1,
                    Humidity = HumidityValue,
                    Pressure = PressureValue,
                    Temperature = TemperatureValue,
                    AirQualityIndex = AirQualityIndexValue,
                    DeviceName = Device2Name
                }
            };

            var result = await repository.GetDeviceReadingsAsync();

            result.Should().BeEquivalentTo(expectedResult.AsQueryable());
        }

        [Test]
        public async Task GetDeviceReadingsAsyncTestEmptyCase()
        {
            var result = await repository.GetDeviceReadingsAsync();

            result.Should().BeEmpty();
        }
        
        private void AddDevicesReadings(DateTimeOffset date1, DateTimeOffset date2)
        {
            dbContext.AqiReadings.AddRange(
                new AqiReading { Timestamp = date1, Value = AirQualityIndexValue, DeviceId = Device1Id },
                new AqiReading { Timestamp = date2, Value = AirQualityIndexValue, DeviceId = Device1Id },
                new AqiReading { Timestamp = date1, Value = AirQualityIndexValue, DeviceId = Device2Id });
            dbContext.PressureReadings.AddRange(
                new PressureReading { Timestamp = date1, Value = PressureValue, DeviceId = Device1Id },
                new PressureReading { Timestamp = date2, Value = PressureValue, DeviceId = Device1Id },
                new PressureReading { Timestamp = date1, Value = PressureValue, DeviceId = Device2Id });
            dbContext.TemperatureReadings.AddRange(
                new TemperatureReading { Timestamp = date1, Value = TemperatureValue, DeviceId = Device1Id },
                new TemperatureReading { Timestamp = date2, Value = TemperatureValue, DeviceId = Device1Id },
                new TemperatureReading { Timestamp = date1, Value = TemperatureValue, DeviceId = Device2Id });
            dbContext.HumidityReadings.AddRange(
                new HumidityReading { Timestamp = date1, Value = HumidityValue, DeviceId = Device1Id },
                new HumidityReading { Timestamp = date2, Value = HumidityValue, DeviceId = Device1Id },
                new HumidityReading { Timestamp = date1, Value = HumidityValue, DeviceId = Device2Id });
            dbContext.SaveChanges();
        }
    }
}