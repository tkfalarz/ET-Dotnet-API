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
        private static readonly Guid DeviceId = new("7353cef6-5456-4c9e-b9ba-a5cf9b141004");
        private static readonly DateTimeOffset Timestamp = DateTimeOffset.Now;
        private IReadingsRepository repository;
        private ApiDbContext context;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            context = TestDatabaseCreator.CreateTestDatabaseContext();
            context.Devices.Add(
                new Device
                {
                    Id = DeviceId,
                    Latitude = "Latitude",
                    Longitude = "Longitude",
                    Name = "Happy device",
                    SensorName = "Happy sensor"
                });
            context.SaveChanges();
        }

        [SetUp]
        public void BeforeEach()
        {
            repository = new ReadingsRepository(context);
        }

        [TearDown]
        public void AfterEach()
        {
            context.AqiReadings.RemoveRange();
            context.HumidityReadings.RemoveRange();
            context.PressureReadings.RemoveRange();
            context.TemperatureReadings.RemoveRange();
            context.SaveChanges();
        }

        [Test]
        public async Task StoreWeatherFactorsAsyncStoresWeatherFactors()
        {
            var weatherReading = new WeatherReading
            {
                Humidity = HumidityValue,
                Pressure = PressureValue,
                Temperature = TemperatureValue,
                AirQualityIndex = AirQualityIndexValue,
                Timestamp = Timestamp
            };

            await repository.StoreWeatherFactorsAsync(weatherReading, DeviceId);

            context.AqiReadings.First(x => x.DeviceId == DeviceId).Should().BeEquivalentTo(
                new AqiReading
                {
                    DeviceId = DeviceId,
                    Timestamp = Timestamp,
                    Value = AirQualityIndexValue
                },
                opt => opt.Excluding(x => x.Device));
            context.HumidityReadings.First(x => x.DeviceId == DeviceId).Should().BeEquivalentTo(
                new HumidityReading
                {
                    DeviceId = DeviceId,
                    Timestamp = Timestamp,
                    Value = HumidityValue
                },
                opt => opt.Excluding(x => x.Device));
            context.TemperatureReadings.First(x => x.DeviceId == DeviceId).Should().BeEquivalentTo(
                new TemperatureReading
                {
                    DeviceId = DeviceId,
                    Timestamp = Timestamp,
                    Value = TemperatureValue
                },
                opt => opt.Excluding(x => x.Device));
            context.PressureReadings.First(x => x.DeviceId == DeviceId).Should().BeEquivalentTo(
                new PressureReading
                {
                    DeviceId = DeviceId,
                    Timestamp = Timestamp,
                    Value = PressureValue
                },
                opt => opt.Excluding(x => x.Device));
        }
    }
}