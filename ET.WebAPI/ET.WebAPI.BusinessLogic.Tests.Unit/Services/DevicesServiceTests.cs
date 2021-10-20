using ET.WebAPI.BusinessLogic.Services;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ET.WebAPI.BusinessLogic.Tests.Unit.Services
{
    [TestFixture]
    public class DevicesServiceTests
    {
        private IDevicesRepository repository;

        [SetUp]
        public void BeforeEach()
        {
            repository = Mock.Of<IDevicesRepository>();
        }

        [Test]
        public void StoreDeviceAsyncThrowsArgumentNullExceptionIfDeviceIsNull()
        {
            var service = CreateService();

            Func<Task> action = async () => await service.StoreDeviceAsync(null);

            action.Should().ThrowAsync<ArgumentNullException>().WithParameterName("device");
        }

        [Test]
        public async Task StoreDeviceAsyncReturnsOperationResultFailureIfDevicesRepositoryThrowsDbUpdateException()
        {
            const string errorMessage = "Some crap happened here";
            Mock.Get(repository).Setup(x => x.StoreDeviceAsync(It.IsAny<Device>())).Throws(new DbUpdateException(errorMessage));
            var service = CreateService();

            var result = await service.StoreDeviceAsync(new Device());

            result.Should().BeOfType<OperationResult>().Which.Should().Be(OperationResult.Failure(errorMessage, ErrorType.Entity));
        }

        [Test]
        public async Task StoreDeviceAsyncReturnsOperationResultProceededIfSucceeded()
        {
            var service = CreateService();

            var result = await service.StoreDeviceAsync(new Device());

            result.Should().BeOfType<OperationResult>().Which.Should().Be(OperationResult.Proceeded());
        }

        [Test]
        public async Task StoreDeviceAsyncCallsDevicesRepositoryForDeviceInsertion()
        {
            var sampleDevice = new Device
            {
                Latitude = "latitude",
                Longitude = "longitude",
                SensorName = "Dummy sensor",
                DeviceName = "Dummy device"
            };
            var service = CreateService();

            await service.StoreDeviceAsync(sampleDevice);

            Mock.Get(repository).Verify(x => x.StoreDeviceAsync(sampleDevice), Times.Once);
        }

        private DevicesService CreateService() => new(repository);
    }
}