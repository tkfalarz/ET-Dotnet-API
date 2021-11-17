using ET.WebAPI.Api.Tests.Integration.Clients;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Database;
using ET.WebAPI.Database.Entities;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ET.WebAPI.Api.Tests.Integration.Tests
{
    [TestFixture]
    public class ReadingsControllerTests
    {
        private const string Device1Name = "Device1";
        private const string Device2Name = "Device2";
        private static readonly Guid Device1Id = Guid.NewGuid();
        private static readonly Guid Device2Id = Guid.NewGuid();
        private ReadingsControllerClient controllerClient;
        private readonly ApiDbContext dbContext;
        private readonly HttpClient httpClient;

        public ReadingsControllerTests()
        {
            controllerClient = new ReadingsControllerClient();
            dbContext = controllerClient.DbContext;
            httpClient = controllerClient.HttpClient;
        }

        [OneTimeSetUp]
        public void BeforeAll()
        {
            dbContext.Devices.AddRange(
                new Device
                {
                    Id = Device1Id,
                    Name = Device1Name,
                    Latitude = 11.1111m,
                    Longitude = 22.2222m,
                    SensorName = "Bme"
                },
                new Device
                {
                    Id = Device2Id,
                    Name = Device2Name,
                    Latitude = 11.1231m,
                    Longitude = 21.3242m,
                    SensorName = "Sensor"
                });
            dbContext.SaveChanges();
        }

        [TearDown]
        public async Task AfterEach()
        {
            dbContext.AqiReadings.RemoveRange(dbContext.AqiReadings);
            dbContext.HumidityReadings.RemoveRange(dbContext.HumidityReadings);
            dbContext.PressureReadings.RemoveRange(dbContext.PressureReadings);
            dbContext.TemperatureReadings.RemoveRange(dbContext.TemperatureReadings);
            await dbContext.SaveChangesAsync();
        }

        [OneTimeTearDown]
        public async Task AfterAll()
        {
            await dbContext.Database.EnsureDeletedAsync();
        }
        
        [Test]
        public async Task StoreReadingsAsyncStoresReadingsForAuthorizedUser()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            
            var result = await controllerClient.StoreReadingAsync(new ReadingView
            {
                Humidity = 12,
                Pressure = 12,
                Temperature = 12,
                Timestamp = dateTimeOffset,
                DeviceName = Device1Name,
                AirQualityIndex = 12
            });

            using var scope = new AssertionScope();
            result.StatusCode.Should().Be(HttpStatusCode.Accepted);
            dbContext.AqiReadings.Where(x => x.Timestamp == dateTimeOffset).Should().HaveCount(1);
            dbContext.HumidityReadings.Where(x => x.Timestamp == dateTimeOffset).Should().HaveCount(1);
            dbContext.TemperatureReadings.Where(x => x.Timestamp == dateTimeOffset).Should().HaveCount(1);
            dbContext.PressureReadings.Where(x => x.Timestamp == dateTimeOffset).Should().HaveCount(1);
        }

        [Test]
        public async Task GetNearestLatestReadingAsyncTest()
        {
            const decimal lat = 11.1110m; //closest to device1 location
            const decimal lon = 22.2220m; //closest to device1 location
            var earliestDate = new DateTimeOffset();
            var latestDate = new DateTimeOffset().AddHours(10);
            AddDevicesReadings(latestDate, earliestDate);
            var expectedResult = new ReadingView
            {
                Humidity = 11,
                Pressure = 11,
                Temperature = 11,
                Timestamp = latestDate,
                DeviceName = Device1Name,
                AirQualityIndex = 11
            };

            var result = await controllerClient.GetNearestLatestReadingAsync(lat, lon);

            using var scope = new AssertionScope();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            ConvertedView<ReadingView>(result.Content).Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetDevicesLatestReadingsAsyncTest()
        {
            var latestDate = DateTimeOffset.Now;
            AddDevicesReadings(latestDate, DateTimeOffset.Now.AddMonths(-2));
            var expectedResult = new ReadingView[]
            {
                new()
                {
                    Timestamp = latestDate,
                    DeviceName = Device1Name,
                    Humidity = 11,
                    Pressure = 11,
                    Temperature = 11,
                    AirQualityIndex = 11
                },
                new()
                {
                    Timestamp = latestDate,
                    DeviceName = Device2Name,
                    Humidity = 21,
                    Pressure = 21,
                    Temperature = 21,
                    AirQualityIndex = 21
                }
            };

            var result = await controllerClient.GetLatestReadingsAsync();

            using var scope = new AssertionScope();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            ConvertedView<ReadingView[]>(result.Content).Should().BeEquivalentTo(expectedResult);
        }
        
        [Test]
        public async Task GetDeviceReadingsAsyncTests()
        {
            var date1 = DateTimeOffset.Now;
            var date2 = DateTimeOffset.Now.AddMonths(1);
            AddDevicesReadings(date1,date2);
            var expectedResult = new[]
            {
                new ReadingView
                {
                    Timestamp = date1,
                    AirQualityIndex = 11,
                    Humidity = 11,
                    Temperature = 11,
                    Pressure = 11,
                    DeviceName = Device1Name
                },
                new ReadingView
                {
                    Timestamp = date2,
                    AirQualityIndex = 12,
                    Humidity = 12,
                    Temperature = 12,
                    Pressure = 12,
                    DeviceName = Device1Name
                    
                }
            };
            var result = await controllerClient.GetDeviceReadingsAsync(Device1Name);

            using var scope = new AssertionScope();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            ConvertedView<ReadingView[]>(result.Content).Should().BeEquivalentTo(expectedResult);
        }

        private void AddDevicesReadings(DateTimeOffset latestDate, DateTimeOffset earliestDate)
        {
            dbContext.AqiReadings.AddRange(
                new AqiReading { Timestamp = latestDate, Value = 11, DeviceId = Device1Id },
                new AqiReading { Timestamp = earliestDate, Value = 12, DeviceId = Device1Id },
                new AqiReading { Timestamp = latestDate, Value = 21, DeviceId = Device2Id });
            dbContext.PressureReadings.AddRange(
                new PressureReading { Timestamp = latestDate, Value = 11, DeviceId = Device1Id },
                new PressureReading { Timestamp = earliestDate, Value = 12, DeviceId = Device1Id },
                new PressureReading { Timestamp = latestDate, Value = 21, DeviceId = Device2Id });
            dbContext.TemperatureReadings.AddRange(
                new TemperatureReading { Timestamp = latestDate, Value = 11, DeviceId = Device1Id },
                new TemperatureReading { Timestamp = earliestDate, Value = 12, DeviceId = Device1Id },
                new TemperatureReading { Timestamp = latestDate, Value = 21, DeviceId = Device2Id });
            dbContext.HumidityReadings.AddRange(
                new HumidityReading { Timestamp = latestDate, Value = 11, DeviceId = Device1Id },
                new HumidityReading { Timestamp = earliestDate, Value = 12, DeviceId = Device1Id },
                new HumidityReading { Timestamp = latestDate, Value = 21, DeviceId = Device2Id });
            dbContext.SaveChanges();
        }

        private static T ConvertedView<T>(HttpContent result) where T : class
            => JsonConvert.DeserializeObject<T>(result.ReadAsStringAsync().GetAwaiter().GetResult());
    }
}