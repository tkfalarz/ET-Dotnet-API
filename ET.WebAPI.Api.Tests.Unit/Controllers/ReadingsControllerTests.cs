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

            var result = await controller.StoreReadingAsync(new ReadingSetView());

            result.Should().BeOfType<BadRequestResult>();
        }

        [Test]
        public async Task StoreReadingAsyncLogsErrorsIfModelStateIsNotValid()
        {
            var controller = new ReadingsController(readingsService, logger);
            controller.ModelState.AddModelError("error", "some error");

            await controller.StoreReadingAsync(new ReadingSetView());

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
                .Setup(x => x.StoreReadingSetAsync(It.IsAny<ReadingSet>()))
                .ReturnsAsync(OperationResult.Failure("some error", ErrorType.Entity));
            var controller = new ReadingsController(readingsService, logger);

            var result = await controller.StoreReadingAsync(new ReadingSetView());

            result.Should().BeEquivalentTo(new StatusCodeResult((int)HttpStatusCode.InternalServerError));
        }

        [Test]
        public async Task StoreReadingAsyncReturnsAcceptedIfOperationCompletedSuccessfully()
        {
            Mock.Get(readingsService)
                .Setup(x => x.StoreReadingSetAsync(It.IsAny<ReadingSet>()))
                .ReturnsAsync(OperationResult.Proceeded);
            var controller = CreateController();

            var result = await controller.StoreReadingAsync(new ReadingSetView());

            result.Should().BeOfType<AcceptedResult>();
        }

        [Test]
        public async Task GetNearestDeviceReadingsAsyncReturnsOkObjectResult()
        {
            const decimal latitude = 70;
            const decimal longitude = 40;
            var expectedResult = new ReadingSetView();
            Mock.Get(readingsService).Setup(x => x.GetNearestLatestReadingsAsync(latitude, longitude)).ReturnsAsync(new ReadingSet());
            var controller = CreateController();

            var result = await controller.GetNearestLatestReadingsSetAsync(latitude, longitude);

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetNearestDeviceReadingsAsyncReturnsNotFoundResult()
        {
            Mock.Get(readingsService).Setup(x => x.GetNearestLatestReadingsAsync(It.IsAny<decimal>(), It.IsAny<decimal>())).ReturnsAsync((ReadingSet)null);
            var controller = CreateController();

            var result = await controller.GetNearestLatestReadingsSetAsync(20m, 20m);

            result.Should().BeOfType<NotFoundResult>();

        }

        private ReadingsController CreateController() => new(readingsService, logger);
    }
}