using ET.WebAPI.Database;
using ET.WebAPI.Database.Entities;
using ET.WebAPI.Database.Repositories;
using ET.WebApi.Database.Tests.Integration.Utilities;
using ET.WebAPI.Kernel.Repositories;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ET.WebApi.Database.Tests.Integration.Repositories
{
    [TestFixture]
    public class DevicesRepositoryTests
    {
        private IDevicesRepository repository;
        private ApiDbContext context;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            context = TestDatabaseCreator.CreateTestDatabaseContext();
        }

        [SetUp]
        public void BeforeEach()
        {
            repository = new DevicesRepository(context);
        }

        [TearDown]
        public void AfterEach()
        {
            context.Devices.RemoveRange(context.Devices);
            context.SaveChanges();
        }

        [Test]
        public async Task GetDevicesReturnsDevices()
        {
            var deviceId = new Guid("3458fa28-9907-43da-9657-13a32be398ec");
            var device2Id = new Guid("4c9cd4ca-bc29-41a4-92ce-f0d4d3ec9211");
            const string deviceName = "Nice device name";
            const string sensorName = "Sensor1";
            const decimal latitude = 12.2212m;
            const decimal longitude = 22.2222m;
            var expectedResult = new[]
            {
                new WebAPI.Kernel.DomainModels.Device
                {
                    DeviceId = deviceId,
                    Latitude = latitude,
                    Longitude = longitude,
                    DeviceName = deviceName,
                    SensorName = sensorName
                },
                new WebAPI.Kernel.DomainModels.Device
                {
                    DeviceId = device2Id,
                    Latitude = latitude,
                    Longitude = longitude,
                    DeviceName = deviceName,
                    SensorName = sensorName
                }
            };
            await context.Devices.AddRangeAsync(
                new Device
                {
                    Id = deviceId,
                    Latitude = latitude,
                    Longitude = longitude,
                    Name = deviceName,
                    SensorName = sensorName
                },
                new Device
                {
                    Id = device2Id,
                    Latitude = latitude,
                    Longitude = longitude,
                    Name = deviceName,
                    SensorName = sensorName
                });
            await context.SaveChangesAsync();
        
            var result = repository.GetDevices();

            result.ToArray().Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task StoreDeviceAsyncStoresTheDeviceIntoDatabase()
        {
            var deviceToInsert = new WebAPI.Kernel.DomainModels.Device
            {
                Latitude = 10.1010m,
                Longitude = 15.2202m,
                SensorName = "Bme680",
                DeviceName = "Device1"
            };
            await repository.StoreDeviceAsync(deviceToInsert);

            context.Devices.Where(
                    x =>
                        x.Name == deviceToInsert.DeviceName
                        && x.SensorName == deviceToInsert.SensorName
                        && x.Latitude == deviceToInsert.Latitude
                        && x.Longitude == deviceToInsert.Longitude)
                .Should().HaveCount(1);
        }
    }
}