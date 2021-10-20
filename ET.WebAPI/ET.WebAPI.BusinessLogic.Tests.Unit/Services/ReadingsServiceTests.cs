using ET.WebAPI.BusinessLogic.Services;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
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
        public async Task StoreWeatherReadingAsyncReturnsOperationResultFailureIfDevicesRepositoryReturnsOperationResultFailure()
        {
            Mock.Get(devicesRepository).Setup(x => x.GetDeviceIdAsync(It.IsAny<string>()))
                .ReturnsAsync(OperationResult<Guid>.Failure(SampleErrorMessage, ErrorType.Entity));
            var service = CreateService();

            var result = await service.StoreWeatherReadingAsync(new DeviceReading());

            result.Should().BeOfType<OperationResult>().Which.Should().Be(OperationResult.Failure(SampleErrorMessage, ErrorType.Entity));
        }

        [Test]
        public async Task StoreWeatherReadingAsyncReturnsOperationResultFailureIfReadingsRepositoryThrowsDbUpdateException()
        {
            Mock.Get(devicesRepository).Setup(x => x.GetDeviceIdAsync(It.IsAny<string>()))
                .ReturnsAsync(OperationResult<Guid>.Proceeded(Guid.NewGuid()));
            Mock.Get(readingsRepository)
                .Setup(x => x.StoreWeatherFactorsAsync(It.IsAny<WeatherReading>(), It.IsAny<Guid>()))
                .ThrowsAsync(new DbUpdateException(SampleErrorMessage));
            var service = CreateService();

            var result = await service.StoreWeatherReadingAsync(new DeviceReading());

            result.Should().BeOfType<OperationResult>().Which.Should().Be(OperationResult.Failure(SampleErrorMessage, ErrorType.Entity));
        }

        [Test]
        public async Task StoreWeatherReadingAsyncReturnsOperationResultProceededIfOperationSucceeded()
        {
            Mock.Get(devicesRepository).Setup(x => x.GetDeviceIdAsync(It.IsAny<string>()))
                .ReturnsAsync(OperationResult<Guid>.Proceeded(Guid.NewGuid()));
            var service = CreateService();

            var result = await service.StoreWeatherReadingAsync(new DeviceReading());

            result.Should().BeOfType<OperationResult>().Which.Should().Be(OperationResult.Proceeded());
        }

        [Test]
        public async Task StoreWeatherReadingAsyncCallsDevicesRepositoryForDeviceExistence()
        {
            Mock.Get(devicesRepository).Setup(x => x.GetDeviceIdAsync(It.IsAny<string>()))
                .ReturnsAsync(OperationResult<Guid>.Proceeded(Guid.NewGuid()));
            var service = CreateService();

            await service.StoreWeatherReadingAsync(new DeviceReading());

            Mock.Get(devicesRepository).Verify(x => x.GetDeviceIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task StoreWeatherReadingAsyncCallsReadingsRepositoryForReadingInsertion()
        {
            var expectedGuid = new Guid("e69c0289-21e3-4b4c-b63a-47d177919142");
            var deviceReading = new DeviceReading
            {
                Humidity = SampleHumidity,
                Pressure = SamplePressure,
                Temperature = SampleTemperature,
                AirQualityIndex = SampleAqi,
                Timestamp = SampleTimestamp
            };
            var expectedVerificationObject = new WeatherReading
            {
                Humidity = SampleHumidity,
                Pressure = SamplePressure,
                Temperature = SampleTemperature,
                AirQualityIndex = SampleAqi,
                Timestamp = SampleTimestamp
            };
            Mock.Get(devicesRepository)
                .Setup(x => x.GetDeviceIdAsync(It.IsAny<string>()))
                .ReturnsAsync(OperationResult<Guid>.Proceeded(expectedGuid));
            var service = CreateService();

            await service.StoreWeatherReadingAsync(deviceReading);

            Mock.Get(readingsRepository).Verify(x => x.StoreWeatherFactorsAsync(expectedVerificationObject, expectedGuid), Times.Once);
        }

        private ReadingsService CreateService() => new(readingsRepository, devicesRepository);
    }
}