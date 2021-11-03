using ET.WebAPI.Api.Tests.Integration.Clients;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Database;
using ET.WebAPI.Database.Entities;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ET.WebAPI.Api.Tests.Integration.Tests
{
    public class DevicesControllerTests
    {
        private const string DeviceName = "RandomRpiName";
        private DevicesControllerClient controllerClient;
        private readonly ApiDbContext dbContext;
        private readonly HttpClient httpClient;

        public DevicesControllerTests()
        {
            controllerClient = new DevicesControllerClient();
            dbContext = controllerClient.DbContext;
            httpClient = controllerClient.HttpClient;
        }

        [OneTimeTearDown]
        public async Task AfterAll()
        {
            await dbContext.Database.EnsureDeletedAsync();
        }

        [Test]
        public async Task StoreDeviceAsyncStoresTheDevice()
        {
            var result = await controllerClient.StoreDeviceAsync(
                new DeviceView
                {
                    DeviceName = DeviceName,
                    Latitude = "Lat",
                    Longitude = "Long",
                    SensorName = "Bme"
                });

            using var scope = new AssertionScope();
            result.StatusCode.Should().Be(HttpStatusCode.Accepted);
            dbContext.Devices.Where(x => x.Name == DeviceName).Should().HaveCount(1);
        }
    }
}