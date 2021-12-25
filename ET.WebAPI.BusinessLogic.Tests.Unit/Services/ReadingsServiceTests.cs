using ET.WebAPI.BusinessLogic.Services;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
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
        private static readonly DateTimeOffset LatestDate = DateTimeOffset.Now;
        private static readonly DateTimeOffset PreviousDate = DateTimeOffset.Now.AddDays(-5);

        private IReadingsRepository readingsRepository;
        private IDevicesRepository devicesRepository;

        [SetUp]
        public void BeforeEach()
        {
            readingsRepository = Mock.Of<IReadingsRepository>();
            devicesRepository = Mock.Of<IDevicesRepository>();

            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(Array.Empty<Device>().AsQueryable().BuildMock().Object);
            
            Mock.Get(readingsRepository)
                .Setup(x => x.GetDeviceReadings())
                .Returns(Array.Empty<ReadingSet>().AsQueryable().BuildMock().Object);
        }

        [Test]
        public void StoreWeatherReadingAsyncThrowsArgumentNullExceptionIfDeviceReadingIsNull()
        {
            var service = CreateService();

            Func<Task> action = async () => await service.StoreReadingSetAsync(null);

            action.Should().ThrowAsync<ArgumentNullException>().WithParameterName("deviceReading");
        }

        [Test]
        public async Task StoreWeatherReadingAsyncReturnsOperationResultFailureIfDevicesRepositoryReturnsNoDevices()
        {
            var expectedResult = OperationResult.Failure($"Device {DeviceName} not found", ErrorType.BusinessLogic);
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(Array.Empty<Device>().AsQueryable().BuildMock().Object);
            var service = CreateService();

            var result = await service.StoreReadingSetAsync(new ReadingSet { DeviceName = DeviceName });

            result.Should().BeOfType<OperationResult>().Which.Should().Be(expectedResult);
        }

        [Test]
        public async Task StoreWeatherReadingAsyncReturnsOperationResultFailureIfReadingsRepositoryThrowsDbUpdateException()
        {
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(new[] { new Device { DeviceId = Guid.NewGuid() } }.AsQueryable().BuildMock().Object);
            Mock.Get(readingsRepository)
                .Setup(x => x.StoreWeatherFactorsAsync(It.IsAny<ReadingSet>(), It.IsAny<Guid>()))
                .ThrowsAsync(new DbUpdateException(SampleErrorMessage));
            var service = CreateService();

            var result = await service.StoreReadingSetAsync(new ReadingSet());

            result.Should().BeOfType<OperationResult>().Which.Should().Be(OperationResult.Failure(SampleErrorMessage, ErrorType.Entity));
        }

        [Test]
        public async Task StoreWeatherReadingAsyncReturnsOperationResultProceededIfOperationSucceeded()
        {
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(new[] { new Device { DeviceId = Guid.NewGuid() } }.AsQueryable().BuildMock().Object);
            var service = CreateService();

            var result = await service.StoreReadingSetAsync(new ReadingSet());

            result.Should().BeOfType<OperationResult>().Which.Should().Be(OperationResult.Proceeded());
        }

        [Test]
        public async Task StoreWeatherReadingAsyncCallsDevicesRepositoryForDeviceExistence()
        {
            var service = CreateService();

            await service.StoreReadingSetAsync(new ReadingSet());

            Mock.Get(devicesRepository).Verify(x => x.GetDevices(), Times.Once);
        }

        [Test]
        public async Task StoreWeatherReadingAsyncCallsReadingsRepositoryForReadingInsertion()
        {
            var expectedGuid = new Guid("e69c0289-21e3-4b4c-b63a-47d177919142");
            var deviceReading = new ReadingSet
            {
                Humidity = SampleHumidity,
                Pressure = SamplePressure,
                Temperature = SampleTemperature,
                AirQualityIndex = SampleAqi,
                Timestamp = SampleTimestamp
            };
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(new[] { new Device { DeviceId = expectedGuid} }.AsQueryable().BuildMock().Object);
            var service = CreateService();

            await service.StoreReadingSetAsync(deviceReading);

            Mock.Get(readingsRepository).Verify(x => x.StoreWeatherFactorsAsync(deviceReading, expectedGuid), Times.Once);
        }

        [Test]
        public async Task GetNearestLatestReadingAsyncCallsDevicesRepositoryForDevices()
        {
            var service = CreateService();

            await service.GetNearestLatestReadingsAsync(21.37m, 2005.2137m);
            
            Mock.Get(devicesRepository).Verify(x=>x.GetDevices(), Times.Once);
        }

        [Test]
        public async Task GetNearestLatestReadingAsyncCallsReadingsRepositoryForReadings()
        {
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(new[] { new Device { Latitude = 1222, Longitude = 1223, DeviceName = DeviceName, SensorName = "Sen1" } }.AsQueryable().BuildMock().Object);
            var service = CreateService();
            
            await service.GetNearestLatestReadingsAsync(21.37m, 2005.2137m);
            
            Mock.Get(readingsRepository).Verify(x=>x.GetDeviceReadings(), Times.Once);
        }

        [Test]
        public async Task GetNearestLatestReadingAsyncNeverCallsReadingsRepositoryIfDevicesRepositoryReturnsNoDevices()
        {
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDevices())
                .Returns(Array.Empty<Device>().AsQueryable().BuildMock().Object);
            var service = CreateService();

            await service.GetNearestLatestReadingsAsync(21.37m, 2005.2137m);

            Mock.Get(readingsRepository).Verify(x => x.GetDeviceReadings(), Times.Never);
        }

        [Test]
        public async Task GetNearestLatestReadingAsyncShouldReturnNullIfEmpty()
        {
            var service = CreateService();

            var result = await service.GetNearestLatestReadingsAsync(0, 0);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetNearestLatestReadingAsyncReturnsReadingWithTheNearestDistanceToGivenCoords()
        {
            var expectedResult = new ReadingSet { DeviceName = DeviceName };
            var service = CreateService();
            Mock.Get(devicesRepository).Setup(x => x.GetDevices()).Returns(
                new[]
                {
                    new Device { DeviceName = DeviceName, Latitude = 10, Longitude = 10 },
                    new Device { DeviceName = "Dev2", Latitude = -11, Longitude = -11 }
                }.AsQueryable().BuildMock().Object);
            Mock.Get(readingsRepository).Setup(x => x.GetDeviceReadings()).Returns(
                new[]
                {
                    new ReadingSet { DeviceName = DeviceName },
                    new ReadingSet { DeviceName = "Dev2" }
                }.AsQueryable().BuildMock().Object);

            var result = await service.GetNearestLatestReadingsAsync(0, 0);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetReadingsAsyncCallsReadingsRepositoryForReadings()
        {
            var service = CreateService();

            await service.GetReadingsAsync("Device1", 0);
            
            Mock.Get(readingsRepository).Verify(x=>x.GetDeviceReadings(), Times.Once);
        }

        [Test]
        public async Task GetReadingsAsyncReturnsEmptyArrayIfNoResult()
        {
            var service = CreateService();

            var result = await service.GetReadingsAsync("Device1", 0);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetReadingsAsyncReturnsList()
        {
            var service = CreateService();

            var result = await service.GetReadingsAsync("Device1", 0);

            result.Should().BeOfType<List<ReadingSet>>();
        }

        [Test]
        public async Task GetReadingsAsyncReturnsLimitedArrayOfReadings()
        {
            var expectedResult = new[] { new ReadingSet { Timestamp = LatestDate, DeviceName = DeviceName } };
            Mock.Get(readingsRepository).Setup(x => x.GetDeviceReadings()).Returns(
                new[]
                {
                    new ReadingSet { Timestamp = LatestDate, DeviceName = DeviceName },
                    new ReadingSet { Timestamp = PreviousDate, DeviceName = DeviceName }
                }.AsQueryable().BuildMock().Object);
            var service = CreateService();

            var result = await service.GetReadingsAsync(DeviceName, 1);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestCase(ReadingType.Temperature)]
        [TestCase(ReadingType.Aqi)]
        [TestCase(ReadingType.Humidity)]
        [TestCase(ReadingType.Pressure)]
        public async Task GetTypedReadingsAsyncReturnsCollectionOfReadings(ReadingType readingType)
        {
            var expectedResult = new Reading[]
            {
                new()
                {
                    Timestamp = LatestDate,
                    Value = 2
                },
                new()
                {
                    Timestamp = PreviousDate,
                    Value = 3
                }
            };
            Mock.Get(readingsRepository)
                .Setup(x => x.GetDeviceReadings())
                .Returns(
                    new[]
                    {
                        new ReadingSet { Timestamp = LatestDate, DeviceName = DeviceName, Humidity = 2, Pressure = 2, Temperature = 2, AirQualityIndex = 2},
                        new ReadingSet { Timestamp = PreviousDate, DeviceName = DeviceName, Humidity = 3, Pressure = 3, Temperature = 3, AirQualityIndex = 3}
                    }.AsQueryable().BuildMock().Object);
            var service = CreateService();

            var result = await service.GetTypedReadingsAsync(DeviceName, readingType, 0);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestCase(ReadingType.Temperature)]
        [TestCase(ReadingType.Aqi)]
        [TestCase(ReadingType.Humidity)]
        [TestCase(ReadingType.Pressure)]
        public async Task GetTypedReadingsAsyncReturnsLimitedCollectionOfReadings(ReadingType readingType)
        {
            var expectedResult = new Reading[] { new() { Timestamp = LatestDate, Value = 2 } };
            Mock.Get(readingsRepository)
                .Setup(x => x.GetDeviceReadings())
                .Returns(
                    new[]
                    {
                        new ReadingSet { Timestamp = LatestDate, DeviceName = DeviceName, Humidity = 2, Pressure = 2, Temperature = 2, AirQualityIndex = 2},
                        new ReadingSet { Timestamp = PreviousDate, DeviceName = DeviceName, Humidity = 3, Pressure = 3, Temperature = 3, AirQualityIndex = 3}
                    }.AsQueryable().BuildMock().Object);
            var service = CreateService();

            var result = await service.GetTypedReadingsAsync(DeviceName, readingType, 1);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestCase(ReadingType.Temperature)]
        [TestCase(ReadingType.Aqi)]
        [TestCase(ReadingType.Humidity)]
        [TestCase(ReadingType.Pressure)]
        public async Task GetTypedReadingsAsyncReturnsEmptyCollectionOfReadingsIfRepositoryYieldsEmptyResult(ReadingType readingType)
        {
            Mock.Get(readingsRepository)
                .Setup(x => x.GetDeviceReadings())
                .Returns(Array.Empty<ReadingSet>().AsQueryable().BuildMock().Object);
            var service = CreateService();

            var result = await service.GetTypedReadingsAsync(DeviceName, readingType, 0);

            result.Should().BeEmpty();
        }

        [TestCase(ReadingType.Temperature)]
        [TestCase(ReadingType.Aqi)]
        [TestCase(ReadingType.Humidity)]
        [TestCase(ReadingType.Pressure)]
        public async Task GetTypedLatestReadingAsyncReturnsLatestReading(ReadingType readingType)
        {
            var expectedResult = new Reading
            {
                Timestamp = LatestDate,
                Value = 2
            };
            Mock.Get(readingsRepository)
                .Setup(x => x.GetDeviceReadings())
                .Returns(
                    new[]
                    {
                        new ReadingSet { Timestamp = LatestDate, DeviceName = DeviceName, Humidity = 2, Pressure = 2, Temperature = 2, AirQualityIndex = 2},
                        new ReadingSet { Timestamp = PreviousDate, DeviceName = DeviceName, Humidity = 3, Pressure = 3, Temperature = 3, AirQualityIndex = 3 }
                    }.AsQueryable().BuildMock().Object);
            var service = CreateService();
            
            var result = await service.GetTypedLatestReadingAsync(DeviceName, readingType);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestCase(ReadingType.Temperature)]
        [TestCase(ReadingType.Aqi)]
        [TestCase(ReadingType.Humidity)]
        [TestCase(ReadingType.Pressure)]
        public async Task GetTypedLatestReadingAsyncReturnsNullIfNoReadingExist(ReadingType readingType)
        {
            Mock.Get(readingsRepository)
                .Setup(x => x.GetDeviceReadings())
                .Returns(Array.Empty<ReadingSet>().AsQueryable().BuildMock().Object);
            var service = CreateService();

            var result = await service.GetTypedLatestReadingAsync(DeviceName, readingType);

            result.Should().BeNull();
        }

        [TestCase(ReadingType.Temperature)]
        [TestCase(ReadingType.Aqi)]
        [TestCase(ReadingType.Humidity)]
        [TestCase(ReadingType.Pressure)]
        public async Task GetTypedLatestReadingsAsyncReturnsNullIfReadingsSetContainsNullValues(ReadingType readingType)
        {
            Mock.Get(readingsRepository)
                .Setup(x => x.GetDeviceReadings())
                .Returns(
                    new[]
                    {
                        new ReadingSet { Timestamp = LatestDate, DeviceName = DeviceName, Humidity = null, Pressure = null, Temperature = null, AirQualityIndex = null},
                        new ReadingSet { Timestamp = PreviousDate, DeviceName = DeviceName, Humidity = null, Pressure = null, Temperature = null, AirQualityIndex = null}
                    }.AsQueryable().BuildMock().Object);
            var service = CreateService();
            
            var result = await service.GetTypedLatestReadingAsync(DeviceName, readingType);

            result.Should().BeNull();
        }
        
        [TestCase(ReadingType.Temperature)]
        [TestCase(ReadingType.Aqi)]
        [TestCase(ReadingType.Humidity)]
        [TestCase(ReadingType.Pressure)]
        public async Task GetTypedLatestReadingsAsyncReturnsLatestKnownReadings(ReadingType readingType)
        {
            var expectedResult = new Reading
            {
                Timestamp = PreviousDate,
                Value = 3
            };
            Mock.Get(readingsRepository)
                .Setup(x => x.GetDeviceReadings())
                .Returns(
                    new[]
                    {
                        new ReadingSet { Timestamp = LatestDate, DeviceName = DeviceName, Humidity = null, Pressure = null, Temperature = null, AirQualityIndex = null },
                        new ReadingSet { Timestamp = PreviousDate, DeviceName = DeviceName, Humidity = 3, Pressure = 3, Temperature = 3, AirQualityIndex = 3 }
                    }.AsQueryable().BuildMock().Object);
            var service = CreateService();
            
            var result = await service.GetTypedLatestReadingAsync(DeviceName, readingType);

            result.Should().BeEquivalentTo(expectedResult);
        }

        private ReadingsService CreateService() => new(readingsRepository, devicesRepository);
    }
}