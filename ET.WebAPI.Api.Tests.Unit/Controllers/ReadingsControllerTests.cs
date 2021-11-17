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
    public class ReadingsControllerTests
    {
        private IReadingsService readingsService;
        private ILogger<ReadingsController> logger;

        [SetUp]
        public void BeforeEach()
        {
            readingsService = Mock.Of<IReadingsService>();
            logger = Mock.Of<ILogger<ReadingsController>>();
        }

        [Test]
        public async Task StoreReadingAsyncReturnsBadRequestIfModelStateIsNotValid()
        {
            var controller = new ReadingsController(readingsService, logger);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.StoreReadingAsync(new ReadingView());

            result.Should().BeOfType<BadRequestResult>();
        }

        [Test]
        public async Task StoreReadingAsyncLogsErrorsIfModelStateIsNotValid()
        {
            var controller = new ReadingsController(readingsService, logger);
            controller.ModelState.AddModelError("error", "some error");

            await controller.StoreReadingAsync(new ReadingView());

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
        public async Task StoreReadingAsyncReturnsInternalServerErrorIfOperationFaulted()
        {
            Mock.Get(readingsService)
                .Setup(x => x.StoreWeatherReadingAsync(It.IsAny<Reading>()))
                .ReturnsAsync(OperationResult.Failure("some error", ErrorType.Entity));
            var controller = new ReadingsController(readingsService, logger);

            var result = await controller.StoreReadingAsync(new ReadingView());

            result.Should().BeEquivalentTo(new StatusCodeResult((int)HttpStatusCode.InternalServerError));
        }

        [Test]
        public async Task StoreReadingAsyncReturnsAcceptedIfOperationCompletedSuccessfully()
        {
            Mock.Get(readingsService)
                .Setup(x => x.StoreWeatherReadingAsync(It.IsAny<Reading>()))
                .ReturnsAsync(OperationResult.Proceeded);
            var controller = new ReadingsController(readingsService, logger);

            var result = await controller.StoreReadingAsync(new ReadingView());

            result.Should().BeOfType<AcceptedResult>();
        }

        [Test]
        public async Task GetLatestReadingsAsyncReturnsOkObjectResultIfSucceeded()
        {
            Mock.Get(readingsService)
                .Setup(x => x.GetLatestReadingsAsync())
                .ReturnsAsync(new List<Reading> { new() });
            var controller = new ReadingsController(readingsService, logger);

            var result = await controller.GetDevicesLatestReadingsAsync();

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<ReadingView[]>();
        }

        [Test]
        public async Task GetLatestReadingsAsyncReturnsNotFoundResult()
        {
            Mock.Get(readingsService)
                .Setup(x => x.GetLatestReadingsAsync())
                .ReturnsAsync(new List<Reading>());
            var controller = new ReadingsController(readingsService, logger);
            
            var result = await controller.GetDevicesLatestReadingsAsync();
            
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetNearestLatestReadingAsyncReturnsOkObjectResultIfSucceeded()
        {
            const decimal lat = 222;
            const decimal lon = 333;
            Mock.Get(readingsService)
                .Setup(x => x.GetNearestLatestReadingAsync(lat,lon))
                .ReturnsAsync(new Reading());
            var controller = new ReadingsController(readingsService, logger);

            var result = await controller.GetNearestLatestReadingAsync(lat, lon);

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<ReadingView>();
        }

        [Test]
        public async Task GetNearestLatestReadingAsyncReturnsNotFoundResult()
        {
            const decimal lat = 222;
            const decimal lon = 333;
            Mock.Get(readingsService)
                .Setup(x => x.GetNearestLatestReadingAsync(lat, lon))
                .ReturnsAsync((Reading)null);
            var controller = new ReadingsController(readingsService, logger);

            var result = await controller.GetNearestLatestReadingAsync(lat, lon);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetDeviceReadingsAsyncReturnsNotFoundResult()
        {
            Mock.Get(readingsService)
                .Setup(x => x.GetDeviceReadingsAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(Array.Empty<Reading>());
            var controller = new ReadingsController(readingsService, logger);

            var result = await controller.GetDeviceReadingsAsync("Device1");

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetDeviceReadingsAsyncReturnsOkObjectResult()
        {
            const string deviceName = "Device1";
            var expectedResult = new[] { new ReadingView() };
            Mock.Get(readingsService)
                .Setup(x => x.GetDeviceReadingsAsync(deviceName, 0))
                .ReturnsAsync(new [] { new Reading() });
            var controller = new ReadingsController(readingsService, logger);

            var result = await controller.GetDeviceReadingsAsync(deviceName);

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(expectedResult);
        }
    }
}