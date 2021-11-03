using ET.WebAPI.Database;
using ET.WebAPI.Database.Entities;
using ET.WebAPI.Database.Repositories;
using ET.WebApi.Database.Tests.Integration.Utilities;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
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
            context.Devices.RemoveRange();
            context.SaveChanges();
        }

        [Test]
        public async Task GetDeviceIdAsyncReturnsDeviceId()
        {
            var deviceId = new Guid("3458fa28-9907-43da-9657-13a32be398ec");
            var deviceName = "Nice device name";
            await context.Devices.AddAsync(
                new Device
                {
                    Id = deviceId,
                    Latitude = "latitude",
                    Longitude = "longitude",
                    Name = deviceName,
                    SensorName = "Bme680"
                });
            await context.SaveChangesAsync();

            var result = await repository.GetDeviceIdAsync(deviceName);

            result.Should().BeOfType<OperationResult<Guid>>().Which.Should().Be(OperationResult<Guid>.Proceeded(deviceId));
        }

        [Test]
        public async Task StoreDeviceAsyncStoresTheDeviceIntoDatabase()
        {
            var deviceToInsert = new WebAPI.Kernel.DomainModels.Device()
            {
                Latitude = "latitude",
                Longitude = "longitude",
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