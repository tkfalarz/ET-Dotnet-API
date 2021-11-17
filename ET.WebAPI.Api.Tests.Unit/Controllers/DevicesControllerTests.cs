using ET.WebAPI.Api.Controllers;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.DomainServices;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ET.WebAPI.Api.Tests.Unit.Controllers
{
    [TestFixture]
    public class DevicesControllerTests
    {
        private IDevicesService devicesService;
        private IDevicesRepository devicesRepository;
        private ILogger<DevicesController> logger;

        [SetUp]
        public void BeforeEach()
        {
            devicesRepository = Mock.Of<IDevicesRepository>();
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
            var mockedData = new[] { new Device() };
            Mock.Get(devicesRepository).Setup(x => x.GetDevices()).Returns(mockedData.AsQueryable().BuildMock().Object);
            var controller = CreateController();

            var result = await controller.GetDevicesAsync();

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<DeviceView[]>();
        }

        [Test]
        public async Task GetDevicesAsyncReturnsNotFoundIfNoDeviceFound()
        {
            Mock.Get(devicesRepository).Setup(x => x.GetDevices()).Returns(Array.Empty<Device>().AsQueryable().BuildMock().Object);
            var controller = CreateController();

            var result = await controller.GetDevicesAsync();

            result.Should().BeOfType<NotFoundResult>();
        }

        private DevicesController CreateController() => new(devicesService, devicesRepository, logger);
    }
}