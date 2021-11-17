using ET.WebAPI.Database;
using ET.WebAPI.Database.Entities;
using ET.WebApi.Database.Tests.Integration.Utilities;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;

namespace ET.WebApi.Database.Tests.Integration.Configuration
{
    [TestFixture]
    public class DeviceConfigurationTests
    {
        private ApiDbContext dbContext;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            dbContext = TestDatabaseCreator.CreateTestDatabaseContext();
        }
        
        [TearDown]
        public void AfterEach()
        {
            dbContext.Devices.RemoveRange(dbContext.Devices);
            dbContext.SaveChanges();
        }
        
        [TestCase(-99.99999)]
        [TestCase(99.99999)]
        public void DeviceHasLatitudeWithProperPrecisionAndScale(decimal latitude)
        {
            dbContext.Devices.Add(
                new Device
                {
                    Latitude = latitude,
                    Longitude = 0,
                    Name = "Dev1",
                    SensorName = "Sen1"
                });
            dbContext.SaveChanges();

            dbContext.Devices.Where(x => x.Latitude == latitude).Should().NotBeEmpty();
        }

        [TestCase(-999.99999)]
        [TestCase(999.99999)]
        public void DeviceHasLongitudeWithProperPrecisionAndScale(decimal longitude)
        {
            dbContext.Devices.Add(
                new Device
                {
                    Latitude = 0,
                    Longitude = longitude,
                    Name = "Dev1",
                    SensorName = "Sen1"
                });
            dbContext.SaveChanges();

            dbContext.Devices.Where(x => x.Longitude == longitude).Should().NotBeEmpty();
        }
    }
}