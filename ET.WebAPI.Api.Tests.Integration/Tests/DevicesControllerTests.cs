using ET.WebAPI.Api.Tests.Integration.Clients;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Database;
using ET.WebAPI.Kernel.DomainModels;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Device = ET.WebAPI.Database.Entities.Device;
using Helper = ET.WebAPI.Api.Tests.Integration.Tools.IntegrationTestsHelper;

namespace ET.WebAPI.Api.Tests.Integration.Tests
{
    public class DevicesControllerTests
    {
        private const string Device2Name = "Device2";
        private static readonly Guid Device2Id = new("d6fa756b-b41f-47fc-bc7b-2b8065caefcf");
        private readonly DevicesControllerClient controllerClient;
        private readonly ApiDbContext dbContext;

        public DevicesControllerTests()
        {
            controllerClient = new DevicesControllerClient();
            dbContext = controllerClient.DbContext;
        }

        [OneTimeTearDown]
        public async Task AfterAll()
        {
            await dbContext.Database.EnsureDeletedAsync();
        }

        [TearDown]
        public async Task AfterEach()
        {
            dbContext.Devices.RemoveRange(dbContext.Devices);
            dbContext.NumericReadings.RemoveRange(dbContext.NumericReadings);
            await dbContext.SaveChangesAsync();
        }

        [Test]
        public async Task StoreDeviceAsyncStoresTheDevice()
        {
            var result = await controllerClient.StoreDeviceAsync(
                new DeviceView
                {
                    DeviceName = Helper.Device1Name,
                    Latitude = Helper.Coordinate,
                    Longitude = 22.2222m,
                    SensorName = "Bme"
                });

            using var scope = new AssertionScope();
            result.StatusCode.Should().Be(HttpStatusCode.Accepted);
            dbContext.Devices.Where(x => x.Name == Helper.Device1Name).Should().HaveCount(1);
        }

        [Test]
        public async Task GetDevicesAsyncTest()
        {
            var expectedResult = new DeviceView[]
            {
                new()
                {
                    DeviceName = Helper.Device1Name,
                    Latitude = Helper.Coordinate,
                    Longitude = Helper.Coordinate,
                    SensorName = Helper.SensorName
                },
                new()
                {
                    DeviceName = Device2Name,
                    Latitude = Helper.Coordinate,
                    Longitude = Helper.Coordinate,
                    SensorName = Helper.SensorName
                }
            };
            dbContext.Devices.AddRange(
                new Device
                {
                    Id = Helper.Device1Id,
                    Name = Helper.Device1Name,
                    Latitude = Helper.Coordinate,
                    Longitude = Helper.Coordinate,
                    SensorName = Helper.SensorName
                },
                new Device
                {
                    Id = Device2Id,
                    Name = Device2Name,
                    Latitude = Helper.Coordinate,
                    Longitude = Helper.Coordinate,
                    SensorName = Helper.SensorName
                });
            await dbContext.SaveChangesAsync();
            
            var result = await controllerClient.GetDevicesAsync();

            Helper.ConvertToObject<DeviceView[]>(result.Content).Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetDeviceReadingsAsyncTest()
        {
            Helper.AddMinimalDatabaseData(dbContext);
            var expectedResult = new ReadingSetView[]
            {
                new() { DeviceName = Helper.Device1Name, Timestamp = Helper.ReadingTimestamp1, Humidity = Helper.ReadingValue, Temperature = Helper.ReadingValue, Pressure = Helper.ReadingValue, AirQualityIndex = Helper.ReadingValue},
                new() { DeviceName = Helper.Device1Name, Timestamp = Helper.ReadingTimestamp2, Humidity = Helper.ReadingValue, Temperature = Helper.ReadingValue, Pressure = Helper.ReadingValue, AirQualityIndex = Helper.ReadingValue}
            };

            var result = await controllerClient.GetDeviceReadingsAsync(Helper.Device1Name);

            Helper.ConvertToObject<ReadingSetView[]>(result.Content).Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetDeviceLatestReadingAsyncTest()
        {
            Helper.AddMinimalDatabaseData(dbContext);
            var expectedResult = new ReadingSetView
            {
                DeviceName = Helper.Device1Name,
                Timestamp = Helper.ReadingTimestamp1,
                Humidity = Helper.ReadingValue,
                Pressure = Helper.ReadingValue,
                Temperature = Helper.ReadingValue,
                AirQualityIndex = Helper.ReadingValue
            };

            var result = await controllerClient.GetDeviceLatestReadingsAsync(Helper.Device1Name);
            
            Helper.ConvertToObject<ReadingSetView>(result.Content).Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetDeviceHumidityReadingsAsyncTest()
        {
            Helper.AddMinimalDatabaseData(dbContext);
            var expectedResult = new ReadingView[]
            {
                new() { Timestamp = Helper.ReadingTimestamp1, Value = Helper.ReadingValue },
                new() { Timestamp = Helper.ReadingTimestamp2, Value = Helper.ReadingValue }
            };

            var result = await controllerClient.GetDeviceWeatherFactorReadingsAsync(Helper.Device1Name, ReadingType.Humidity);
            
            Helper.ConvertToObject<ReadingView[]>(result.Content).Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetDeviceLatestHumidityReadingAsyncTest()
        {
            Helper.AddMinimalDatabaseData(dbContext);
            var expectedResult = new ReadingView { Timestamp = Helper.ReadingTimestamp1, Value = Helper.ReadingValue };
            
            var result = await controllerClient.GetDeviceLatestWeatherFactorReadingsAsync(Helper.Device1Name, ReadingType.Humidity);
            
            Helper.ConvertToObject<ReadingView>(result.Content).Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetDevicePressureReadingsAsyncTest()
        {
            Helper.AddMinimalDatabaseData(dbContext);
            var expectedResult = new ReadingView[]
            {
                new() { Timestamp = Helper.ReadingTimestamp1, Value = Helper.ReadingValue },
                new() { Timestamp = Helper.ReadingTimestamp2, Value = Helper.ReadingValue }
            };

            var result = await controllerClient.GetDeviceWeatherFactorReadingsAsync(Helper.Device1Name, ReadingType.Pressure);
            Helper.ConvertToObject<ReadingView[]>(result.Content).Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetDeviceLatestPressureReadingAsyncTest()
        {
            Helper.AddMinimalDatabaseData(dbContext);
            var expectedResult = new ReadingView { Timestamp = Helper.ReadingTimestamp1, Value = Helper.ReadingValue };
            
            var result = await controllerClient.GetDeviceLatestWeatherFactorReadingsAsync(Helper.Device1Name, ReadingType.Pressure);
            
            Helper.ConvertToObject<ReadingView>(result.Content).Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetDeviceTemperatureReadingsTest()
        {
            Helper.AddMinimalDatabaseData(dbContext);
            var expectedResult = new ReadingView[]
            {
                new() { Timestamp = Helper.ReadingTimestamp1, Value = Helper.ReadingValue },
                new() { Timestamp = Helper.ReadingTimestamp2, Value = Helper.ReadingValue }
            };

            var result = await controllerClient.GetDeviceWeatherFactorReadingsAsync(Helper.Device1Name, ReadingType.Temperature);
            Helper.ConvertToObject<ReadingView[]>(result.Content).Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetDeviceLatestTemperatureReadingTest()
        {
            Helper.AddMinimalDatabaseData(dbContext);
            var expectedResult = new ReadingView { Timestamp = Helper.ReadingTimestamp1, Value = Helper.ReadingValue };
            
            var result = await controllerClient.GetDeviceLatestWeatherFactorReadingsAsync(Helper.Device1Name, ReadingType.Temperature);
            
            Helper.ConvertToObject<ReadingView>(result.Content).Should().BeEquivalentTo(expectedResult);
        }
        
        [Test]
        public async Task GetDeviceAirQualityIndexReadingsTest()
        {
            Helper.AddMinimalDatabaseData(dbContext);
            var expectedResult = new ReadingView[]
            {
                new() { Timestamp = Helper.ReadingTimestamp1, Value = Helper.ReadingValue },
                new() { Timestamp = Helper.ReadingTimestamp2, Value = Helper.ReadingValue }
            };

            var result = await controllerClient.GetDeviceWeatherFactorReadingsAsync(Helper.Device1Name, ReadingType.Aqi);
            Helper.ConvertToObject<ReadingView[]>(result.Content).Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetDeviceLatestAirQualityIndexReadingTest()
        {
            Helper.AddMinimalDatabaseData(dbContext);
            var expectedResult = new ReadingView { Timestamp = Helper.ReadingTimestamp1, Value = Helper.ReadingValue };
            
            var result = await controllerClient.GetDeviceLatestWeatherFactorReadingsAsync(Helper.Device1Name, ReadingType.Aqi);
            
            Helper.ConvertToObject<ReadingView>(result.Content).Should().BeEquivalentTo(expectedResult);
        }
    }
}