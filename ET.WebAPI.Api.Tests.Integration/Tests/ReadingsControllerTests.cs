using ET.WebAPI.Api.Tests.Integration.Clients;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Database;
using ET.WebAPI.Database.Entities;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ET.WebAPI.Api.Tests.Integration.Tests
{
    [TestFixture]
    public class ReadingsControllerTests
    {
        private const string DeviceName = "RandomRpiName";
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
            dbContext.Devices.Add(
                new Device
                {
                    Id = Guid.NewGuid(),
                    Name = DeviceName,
                    Latitude = "Lat",
                    Longitude = "Long",
                    SensorName = "Bme"
                });
            dbContext.SaveChanges();
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
            var result = await controllerClient.StoreReadingAsync(new DeviceReadingView
            {
                Humidity = 12,
                Pressure = 12,
                Temperature = 12,
                Timestamp = dateTimeOffset,
                DeviceName = DeviceName,
                AirQualityIndex = 12
            });

            using var scope = new AssertionScope();
            result.StatusCode.Should().Be(HttpStatusCode.Accepted);
            dbContext.AqiReadings.Where(x => x.Timestamp == dateTimeOffset).Should().HaveCount(1);
            dbContext.HumidityReadings.Where(x => x.Timestamp == dateTimeOffset).Should().HaveCount(1);
            dbContext.TemperatureReadings.Where(x => x.Timestamp == dateTimeOffset).Should().HaveCount(1);
            dbContext.PressureReadings.Where(x => x.Timestamp == dateTimeOffset).Should().HaveCount(1);
        }
    }
}