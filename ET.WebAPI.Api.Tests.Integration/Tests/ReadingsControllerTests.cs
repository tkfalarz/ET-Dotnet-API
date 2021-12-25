using ET.WebAPI.Api.Tests.Integration.Clients;
using ET.WebAPI.Api.Tests.Integration.Tools;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Database;
using ET.WebAPI.Database.Entities;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
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
        private readonly ReadingsControllerClient controllerClient;
        private readonly ApiDbContext dbContext;

        public ReadingsControllerTests()
        {
            controllerClient = new ReadingsControllerClient();
            dbContext = controllerClient.DbContext;
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
            dbContext.NumericReadings.RemoveRange(dbContext.NumericReadings);
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
            
            var result = await controllerClient.StoreReadingAsync(new ReadingSetView
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
            dbContext.NumericReadings.Where(x => x.Timestamp == dateTimeOffset && x.ReadingType == NumericReadingType.AirQualityIndex).Should().HaveCount(1);
            dbContext.NumericReadings.Where(x => x.Timestamp == dateTimeOffset && x.ReadingType == NumericReadingType.Temperature).Should().HaveCount(1);
            dbContext.NumericReadings.Where(x => x.Timestamp == dateTimeOffset && x.ReadingType == NumericReadingType.Humidity).Should().HaveCount(1);
            dbContext.NumericReadings.Where(x => x.Timestamp == dateTimeOffset && x.ReadingType == NumericReadingType.Pressure).Should().HaveCount(1);
        }

        [Test]
        public async Task GetNearestLatestReadingsAsyncTest()
        {
            const decimal lat = 11.1110m; //closest to device1 location
            const decimal lon = 22.2220m; //closest to device1 location
            var earliestDate = new DateTimeOffset();
            var latestDate = new DateTimeOffset().AddHours(10);
            AddDevicesReadings(latestDate, earliestDate);
            var expectedResult = new ReadingSetView
            {
                Humidity = 11,
                Pressure = 11,
                Temperature = 11,
                Timestamp = latestDate,
                DeviceName = Device1Name,
                AirQualityIndex = 11
            };

            var result = await controllerClient.GetNearestLatestReadingsAsync(lat, lon);

            using var scope = new AssertionScope();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            IntegrationTestsHelper.ConvertToObject<ReadingSetView>(result.Content).Should().BeEquivalentTo(expectedResult);
        }
        
        private void AddDevicesReadings(DateTimeOffset latestDate, DateTimeOffset earliestDate)
        {
            dbContext.NumericReadings.AddRange(
                new NumericReading { ReadingType = NumericReadingType.AirQualityIndex,Timestamp = latestDate, Value = 11, DeviceId = Device1Id },
                new NumericReading { ReadingType = NumericReadingType.AirQualityIndex,Timestamp = earliestDate, Value = 12, DeviceId = Device1Id },
                new NumericReading { ReadingType = NumericReadingType.AirQualityIndex,Timestamp = latestDate, Value = 21, DeviceId = Device2Id });
            dbContext.NumericReadings.AddRange(
                new NumericReading { ReadingType = NumericReadingType.Pressure, Timestamp = latestDate, Value = 11, DeviceId = Device1Id },
                new NumericReading { ReadingType = NumericReadingType.Pressure, Timestamp = earliestDate, Value = 12, DeviceId = Device1Id },
                new NumericReading { ReadingType = NumericReadingType.Pressure, Timestamp = latestDate, Value = 21, DeviceId = Device2Id });
            dbContext.NumericReadings.AddRange(
                new NumericReading { ReadingType = NumericReadingType.Temperature, Timestamp = latestDate, Value = 11, DeviceId = Device1Id },
                new NumericReading { ReadingType = NumericReadingType.Temperature, Timestamp = earliestDate, Value = 12, DeviceId = Device1Id },
                new NumericReading { ReadingType = NumericReadingType.Temperature, Timestamp = latestDate, Value = 21, DeviceId = Device2Id });
            dbContext.NumericReadings.AddRange(
                new NumericReading { ReadingType = NumericReadingType.Humidity, Timestamp = latestDate, Value = 11, DeviceId = Device1Id },
                new NumericReading { ReadingType = NumericReadingType.Humidity, Timestamp = earliestDate, Value = 12, DeviceId = Device1Id },
                new NumericReading { ReadingType = NumericReadingType.Humidity, Timestamp = latestDate, Value = 21, DeviceId = Device2Id });
            dbContext.SaveChanges();
        }
    }
}