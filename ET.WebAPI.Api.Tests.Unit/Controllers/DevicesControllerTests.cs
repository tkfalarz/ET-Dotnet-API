using ET.WebAPI.Api.Controllers;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.DomainServices;
using ET.WebAPI.Kernel.ErrorsHandling;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ET.WebAPI.Api.Tests.Unit.Controllers
{
    [TestFixture]
    public class DevicesControllerTests
    {
        private const string LimitCriteriaBelowValueErrorMessage = "Limit criteria below 0 value";
        private const string FactorNotSupportedErrorMessage = "This type of weather factor is not supported";
        private IDevicesService devicesService;
        private IReadingsService readingsService;
        private ILogger<DevicesController> logger;

        [SetUp]
        public void BeforeEach()
        {
            readingsService = Mock.Of<IReadingsService>();
            devicesService = Mock.Of<IDevicesService>();
            logger = Mock.Of<ILogger<DevicesController>>();
        }
        
        
        [Test]
        public async Task StoreDeviceAsyncReturnsBadRequestIfModelStateIsNotValid()
        {
            var controller = CreateController();
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.StoreDeviceAsync(new DeviceView());

            result.Should().BeOfType<BadRequestResult>();
        }

        [Test]
        public async Task StoreDeviceAsyncLogsErrorsIfModelStateIsNotValid()
        {
            var controller = CreateController();
            controller.ModelState.AddModelError("error", "some error");

            await controller.StoreDeviceAsync(new DeviceView());
            
            Mock.Get(logger).Verify(
                m => m.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task StoreDeviceAsyncReturnsInternalServerErrorIfOperationFaulted()
        {
            Mock.Get(devicesService)
                .Setup(x => x.StoreDeviceAsync(It.IsAny<Device>()))
                .ReturnsAsync(OperationResult.Failure("some error message", ErrorType.Entity));
            var controller = CreateController();
            
            var result = await controller.StoreDeviceAsync(new DeviceView());

            result.Should().BeEquivalentTo(new StatusCodeResult((int)HttpStatusCode.InternalServerError));
        }
        
        [Test]
        public async Task StoreDeviceAsyncReturnsAcceptedIfOperationCompletedSuccessfully()
        {
            Mock.Get(devicesService)
                .Setup(x => x.StoreDeviceAsync(It.IsAny<Device>()))
                .ReturnsAsync(OperationResult.Proceeded);
            var controller = CreateController();
            
            var result = await controller.StoreDeviceAsync(new DeviceView());

            result.Should().BeOfType<AcceptedResult>();
        }

        [Test]
        public async Task GetDevicesAsyncReturnsOkIfSucceeded()
        {
            Mock.Get(devicesService).Setup(x => x.GetDevicesAsync()).ReturnsAsync(new List<Device>{new()});
            var controller = CreateController();

            var result = await controller.GetDevicesAsync();

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<List<DeviceView>>();
        }

        [Test]
        public async Task GetDevicesAsyncReturnsNotFoundIfNoDeviceFound()
        {
            Mock.Get(devicesService).Setup(x => x.GetDevicesAsync()).ReturnsAsync(new List<Device>());
            var controller = CreateController();

            var result = await controller.GetDevicesAsync();

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetDeviceAsyncReturnsNotFound()
        {
            const string device = "dev1";
            Mock.Get(devicesService).Setup(x => x.GetDeviceAsync(device)).ReturnsAsync((Device)null);
            var controller = CreateController();

            var result = await controller.GetDeviceAsync(device);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetDeviceAsyncReturnsOkObjectResult()
        {
            const string device = "dev1";
            Mock.Get(devicesService).Setup(x => x.GetDeviceAsync(device)).ReturnsAsync(new Device());
            var controller = CreateController();

            var result = await controller.GetDeviceAsync(device);

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<DeviceView>();
        }

        [Test]
        public async Task GetDeviceReadingsAsyncReturnsNotFoundIfReadingsSetCollectionIsNull()
        {
            const string deviceName = "dev1";
            Mock.Get(readingsService)
                .Setup(x => x.GetReadingsAsync(deviceName, 0))
                .ReturnsAsync((List<ReadingSet>)null);
            var controller = CreateController();

            var result = await controller.GetDeviceReadingsAsync(deviceName, 0);

            result.Should().BeOfType<NotFoundResult>();
        }
        
        [Test]
        public async Task GetDeviceReadingsAsyncReturnsNotFoundIfReadingsSetCollectionIsEmpty()
        {
            const string deviceName = "dev1";
            Mock.Get(readingsService)
                .Setup(x => x.GetReadingsAsync(deviceName, 0))
                .ReturnsAsync(new List<ReadingSet>());
            var controller = CreateController();

            var result = await controller.GetDeviceReadingsAsync(deviceName, 0);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetDeviceReadingsAsyncReturnsOkObjectResult()
        {
            var expectedResult = new List<ReadingSet> { new() };
            const string deviceName = "dev1";
            Mock.Get(readingsService)
                .Setup(x => x.GetReadingsAsync(deviceName, 0))
                .ReturnsAsync(expectedResult);
            var controller = CreateController();

            var result = await controller.GetDeviceReadingsAsync(deviceName, 0);

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetDeviceLatestReadingsAsyncReturnsOkObjectResultIfSucceeded()
        {
            const string deviceName = "dev1";
            Mock.Get(readingsService)
                .Setup(x => x.GetLatestReadingsAsync(deviceName))
                .ReturnsAsync(new ReadingSet());
            var controller = CreateController();

            var result = await controller.GetDeviceLatestReadingsAsync(deviceName);

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<ReadingSetView>();
        }

        [Test]
        public async Task GetDeviceLatestReadingsAsyncReturnsNotFoundResult()
        {
            const string deviceName = "dev1";
            Mock.Get(readingsService)
                .Setup(x => x.GetLatestReadingsAsync(deviceName))
                .ReturnsAsync((ReadingSet)null);
            var controller = CreateController();
            
            var result = await controller.GetDeviceLatestReadingsAsync(deviceName);
            
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestCase(ReadingType.Aqi)]
        [TestCase(ReadingType.Temperature)]
        [TestCase(ReadingType.Humidity)]
        [TestCase(ReadingType.Pressure)]
        public async Task GetDeviceWeatherFactorReadingsAsyncReturnsNotFoundIfReadingsCollectionIsNull(ReadingType readingType)
        {
            const string device = "dev";
            Mock.Get(readingsService)
                .Setup(x => x.GetTypedReadingsAsync(device, readingType, 0))
                .ReturnsAsync((IReadOnlyList<Reading>)null);
            var controller = CreateController();

            var result = await controller.GetDeviceWeatherFactorReadingsAsync(device, readingType,0);

            result.Should().BeOfType<NotFoundResult>();
        }
        
        [TestCase(ReadingType.Aqi)]
        [TestCase(ReadingType.Temperature)]
        [TestCase(ReadingType.Humidity)]
        [TestCase(ReadingType.Pressure)]
        public async Task GetDeviceWeatherFactorReadingsAsyncReturnsNotFoundIfReadingsCollectionIsEmpty(ReadingType readingType)
        {
            const string device = "dev";
            Mock.Get(readingsService)
                .Setup(x => x.GetTypedReadingsAsync(device, readingType, 0))
                .ReturnsAsync(new List<Reading>());
            var controller = CreateController();

            var result = await controller.GetDeviceWeatherFactorReadingsAsync(device, readingType, 0);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetDeviceWeatherFactorReadingsAsyncReturnsBadRequestIfInvalidReadingTypeProvided()
        {
            var controller = CreateController();

            var result = await controller.GetDeviceWeatherFactorReadingsAsync("device", (ReadingType)69, 0);

            result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be(FactorNotSupportedErrorMessage);
        }
        
        [TestCase(ReadingType.Aqi)]
        [TestCase(ReadingType.Temperature)]
        [TestCase(ReadingType.Humidity)]
        [TestCase(ReadingType.Pressure)]
        public async Task GetDeviceWeatherFactorReadingsAsyncReturnsBadRequestIfInvalidLimitProvided(ReadingType readingType)
        {
            var controller = CreateController();

            var result = await controller.GetDeviceWeatherFactorReadingsAsync("device", readingType, -1);

            result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be(LimitCriteriaBelowValueErrorMessage);
        }

        [TestCase(ReadingType.Aqi)]
        [TestCase(ReadingType.Temperature)]
        [TestCase(ReadingType.Humidity)]
        [TestCase(ReadingType.Pressure)]
        public async Task GetDeviceWeatherFactorReadingsAsyncReturnsOkObjectResult(ReadingType readingType)
        {
            const string device = "dev";
            Mock.Get(readingsService)
                .Setup(x => x.GetTypedReadingsAsync(device, readingType, 0))
                .ReturnsAsync(new List<Reading> { new() });
            var controller = CreateController();

            var result = await controller.GetDeviceWeatherFactorReadingsAsync(device, readingType, 0);

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<List<ReadingView>>();

        }
        
        [TestCase(ReadingType.Aqi)]
        [TestCase(ReadingType.Temperature)]
        [TestCase(ReadingType.Humidity)]
        [TestCase(ReadingType.Pressure)]
        public async Task GetDeviceLatestWeatherFactorReadingsAsyncReturnsNotFoundIfReadingIsNull(ReadingType readingType)
        {
            const string device = "dev1";
            Mock.Get(readingsService)
                .Setup(x => x.GetTypedLatestReadingAsync(device, readingType))
                .ReturnsAsync((Reading)null);
            var controller = CreateController();

            var result = await controller.GetDeviceWeatherFactorLatestReadingAsync(device, readingType);

            result.Should().BeOfType<NotFoundResult>();
        }

        [TestCase(ReadingType.Aqi)]
        [TestCase(ReadingType.Temperature)]
        [TestCase(ReadingType.Humidity)]
        [TestCase(ReadingType.Pressure)]
        public async Task GetDeviceLatestWeatherFactorReadingsAsyncReturnsOkObjectResult(ReadingType readingType)
        {
            const string device = "dev1";
            Mock.Get(readingsService)
                .Setup(x => x.GetTypedLatestReadingAsync(device, readingType))
                .ReturnsAsync(new Reading());
            var controller = CreateController();

            var result = await controller.GetDeviceWeatherFactorLatestReadingAsync(device, readingType);

            result.Should().BeOfType<OkObjectResult>();
        }

        private DevicesController CreateController() => new(devicesService, readingsService, logger);
    }
}