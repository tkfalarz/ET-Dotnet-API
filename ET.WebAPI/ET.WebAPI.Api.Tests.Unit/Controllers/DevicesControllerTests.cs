using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using ET.WebAPI.Api.Controllers;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.DomainServices;
using ET.WebAPI.Kernel.ErrorsHandling;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ET.WebAPI.Api.Tests.Unit.Controllers
{
    [TestFixture]
    public class DevicesControllerTests
    {
        private IDevicesService devicesService;
        private ILogger<DevicesController> logger;

        [SetUp]
        public void BeforeEach()
        {
            devicesService = Mock.Of<IDevicesService>();
            logger = Mock.Of<ILogger<DevicesController>>();
        }
        
        
        [Test]
        public async Task StoreDeviceAsyncReturnsBadRequestIfModelStateIsNotValid()
        {
            var controller = new DevicesController(devicesService, logger);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.StoreDeviceAsync(new DeviceView());

            result.Should().BeOfType<BadRequestResult>();
        }

        [Test]
        public async Task StoreDeviceAsyncLogsErrorsIfModelStateIsNotValid()
        {
            var controller = new DevicesController(devicesService, logger);
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
            var controller = new DevicesController(devicesService, logger);
            
            var result = await controller.StoreDeviceAsync(new DeviceView());

            result.Should().BeEquivalentTo(new StatusCodeResult((int)HttpStatusCode.InternalServerError));
        }
        
        [Test]
        public async Task StoreDeviceAsyncReturnsAcceptedIfOperationCompletedSuccessfully()
        {
            Mock.Get(devicesService)
                .Setup(x => x.StoreDeviceAsync(It.IsAny<Device>()))
                .ReturnsAsync(OperationResult.Proceeded);
            var controller = new DevicesController(devicesService, logger);
            
            var result = await controller.StoreDeviceAsync(new DeviceView());

            result.Should().BeOfType<AcceptedResult>();
        }
    }
}