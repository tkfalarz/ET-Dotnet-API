using ET.WebAPI.BusinessLogic.Services;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ET.WebAPI.BusinessLogic.Tests.Unit.Services
{
    [TestFixture]
    public class ReadingsServiceTests
    {
        private const double SampleHumidity = 21;
        private const double SamplePressure = 22;
        private const double SampleTemperature = 23;
        private const double SampleAqi = 24;
        private const string SampleErrorMessage = "some crap happened here...";
        private const string DeviceName = "dev1";
        private static readonly DateTimeOffset SampleTimestamp = DateTimeOffset.Now;

        private IReadingsRepository readingsRepository;
        private IDevicesRepository devicesRepository;

        [SetUp]
        public void BeforeEach()
        {
            readingsRepository = Mock.Of<IReadingsRepository>();
            devicesRepository = Mock.Of<IDevicesRepository>();
        }

        [Test]
        public void StoreWeatherReadingAsyncThrowsArgumentNullExceptionIfDeviceReadingIsNull()
        {
            var service = CreateService();

            Func<Task> action = async () => await service.StoreWeatherReadingAsync(null);

            action.Should().ThrowAsync<ArgumentNullException>().WithParameterName("deviceReading");
        }

        [Test]
        public async Task StoreWeatherReadingAsyncReturnsOperationResultFailureIfDevicesRepositoryReturnsNoDevices()
        {
            var expectedResult = OperationResult.Failure($"Device {DeviceName} not found", ErrorType.BusinessLogic);
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(Array.Empty<Device>().AsQueryable);
            var service = CreateService();

            var result = await service.StoreWeatherReadingAsync(new Reading { DeviceName = DeviceName });

            result.Should().BeOfType<OperationResult>().Which.Should().Be(expectedResult);
        }

        [Test]
        public async Task StoreWeatherReadingAsyncReturnsOperationResultFailureIfReadingsRepositoryThrowsDbUpdateException()
        {
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(new[] { new Device { DeviceId = Guid.NewGuid() } }.AsQueryable);
            Mock.Get(readingsRepository)
                .Setup(x => x.StoreWeatherFactorsAsync(It.IsAny<Reading>(), It.IsAny<Guid>()))
                .ThrowsAsync(new DbUpdateException(SampleErrorMessage));
            var service = CreateService();

            var result = await service.StoreWeatherReadingAsync(new Reading());

            result.Should().BeOfType<OperationResult>().Which.Should().Be(OperationResult.Failure(SampleErrorMessage, ErrorType.Entity));
        }

        [Test]
        public async Task StoreWeatherReadingAsyncReturnsOperationResultProceededIfOperationSucceeded()
        {
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(new[] { new Device { DeviceId = Guid.NewGuid() } }.AsQueryable);
            var service = CreateService();

            var result = await service.StoreWeatherReadingAsync(new Reading());

            result.Should().BeOfType<OperationResult>().Which.Should().Be(OperationResult.Proceeded());
        }

        [Test]
        public async Task StoreWeatherReadingAsyncCallsDevicesRepositoryForDeviceExistence()
        {
            var service = CreateService();

            await service.StoreWeatherReadingAsync(new Reading());

            Mock.Get(devicesRepository).Verify(x => x.GetDevices(), Times.Once);
        }

        [Test]
        public async Task StoreWeatherReadingAsyncCallsReadingsRepositoryForReadingInsertion()
        {
            var expectedGuid = new Guid("e69c0289-21e3-4b4c-b63a-47d177919142");
            var deviceReading = new Reading
            {
                Humidity = SampleHumidity,
                Pressure = SamplePressure,
                Temperature = SampleTemperature,
                AirQualityIndex = SampleAqi,
                Timestamp = SampleTimestamp
            };
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(new[] { new Device { DeviceId = expectedGuid} }.AsQueryable);
            var service = CreateService();

            await service.StoreWeatherReadingAsync(deviceReading);

            Mock.Get(readingsRepository).Verify(x => x.StoreWeatherFactorsAsync(deviceReading, expectedGuid), Times.Once);
        }

        [Test]
        public async Task GetLatestReadingsAsyncReturnsListOfReadingsResult()
        {
            var service = CreateService();

            var result = await service.GetLatestReadingsAsync();

            result.Should().BeOfType<List<Reading>>();
        }
        
        [Test]
        public async Task GetLatestReadingsAsyncCallsDeviceRepositoryForDevices()
        {
            var service = CreateService();

            await service.GetLatestReadingsAsync();

            Mock.Get(devicesRepository).Verify(x => x.GetDevices(), Times.Once);
        }

        [Test]
        public async Task GetLatestReadingsAsyncNeverCallsReadingsRepositoryForReadingsIfDeviceRepositoryNotReturnAnyDevice()
        {
            var service = CreateService();
            
            await service.GetLatestReadingsAsync();

            Mock.Get(readingsRepository).Verify(x => x.GetDeviceReadingsAsync(), Times.Never);
        }

        [Test]
        public async Task GetLatestReadingsAsyncCallsReadingsRepositoryForReadings()
        {
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(new[] { new Device { Latitude = 1222, Longitude = 1223, DeviceName = "Dev1", SensorName = "Sen1" } }.AsQueryable());
            var service = CreateService();

            await service.GetLatestReadingsAsync();

            Mock.Get(readingsRepository).Verify(x => x.GetDeviceReadingsAsync(), Times.Once);
        }

        [Test]
        public async Task GetNearestLatestReadingAsyncCallsDevicesRepositoryForDevices()
        {
            var service = CreateService();

            await service.GetNearestLatestReadingAsync(21.37m, 2005.2137m);
            
            Mock.Get(devicesRepository).Verify(x=>x.GetDevices(), Times.Once);
        }

        [Test]
        public async Task GetNearestLatestReadingAsyncCallsReadingsRepositoryForReadings()
        {
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(new[] { new Device { Latitude = 1222, Longitude = 1223, DeviceName = DeviceName, SensorName = "Sen1" } }.AsQueryable());
            var service = CreateService();
            
            await service.GetNearestLatestReadingAsync(21.37m, 2005.2137m);
            
            Mock.Get(readingsRepository).Verify(x=>x.GetDeviceReadingsAsync(), Times.Once);
        }

        [Test]
        public async Task GetNearestLatestReadingAsyncNeverCallsReadingsRepositoryIfDevicesRepositoryReturnsNoDevices()
        {
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(Array.Empty<Device>().AsQueryable);
            var service = CreateService();

            await service.GetNearestLatestReadingAsync(21.37m, 2005.2137m);

            Mock.Get(readingsRepository).Verify(x => x.GetDeviceReadingsAsync(), Times.Never);
        }

        [Test]
        public async Task GetNearestLatestReadingAsyncShouldReturnNullIfEmpty()
        {
            var service = CreateService();

            var result = await service.GetNearestLatestReadingAsync(0, 0);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetNearestLatestReadingAsyncReturnsReadingWithTheNearestDistanceToGivenCoords()
        {
            var expectedResult = new Reading { DeviceName = DeviceName };
            var service = CreateService();
            Mock.Get(devicesRepository).Setup(x => x.GetDevices()).Returns(
                new[]
                {
                    new Device { DeviceName = DeviceName, Latitude = 10, Longitude = 10 },
                    new Device { DeviceName = "Dev2", Latitude = -11, Longitude = -11 }
                }.AsQueryable());
            Mock.Get(readingsRepository).Setup(x => x.GetDeviceReadingsAsync()).ReturnsAsync(
                new[]
                {
                    new Reading { DeviceName = DeviceName },
                    new Reading { DeviceName = "Dev2" }
                }.AsQueryable);

            var result = await service.GetNearestLatestReadingAsync(0, 0);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetDeviceReadingsAsyncCallsReadingsRepositoryForReadings()
        {
            var service = CreateService();

            await service.GetDeviceReadingsAsync("Device1", 0);
            
            Mock.Get(readingsRepository).Verify(x=>x.GetDeviceReadingsAsync(), Times.Once);
        }

        [Test]
        public async Task GetDeviceReadingsAsyncReturnsEmptyArrayIfNoResult()
        {
            var service = CreateService();

            var result = await service.GetDeviceReadingsAsync("Device1", 0);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetDeviceReadingsAsyncReturnsArrayOfReadings()
        {
            var service = CreateService();

            var result = await service.GetDeviceReadingsAsync("Device1", 0);

            result.Should().BeOfType<Reading[]>();
        }

        [Test]
        public async Task GetDeviceReadingsAsyncReturnsLimitedArrayOfReadings()
        {
            var latestDate = DateTimeOffset.Now;
            var previousDate = DateTimeOffset.Now.AddDays(-5);
            var expectedResult = new[] { new Reading { Timestamp = latestDate, DeviceName = DeviceName } };
            Mock.Get(readingsRepository).Setup(x => x.GetDeviceReadingsAsync()).ReturnsAsync(
                new[]
                {
                    new Reading { Timestamp = latestDate, DeviceName = DeviceName },
                    new Reading { Timestamp = previousDate, DeviceName = DeviceName }
                }.AsQueryable);
            var service = CreateService();

            var result = await service.GetDeviceReadingsAsync(DeviceName, 1);

            result.Should().BeEquivalentTo(expectedResult);
        }
        
        private ReadingsService CreateService() => new(readingsRepository, devicesRepository);
    }
}