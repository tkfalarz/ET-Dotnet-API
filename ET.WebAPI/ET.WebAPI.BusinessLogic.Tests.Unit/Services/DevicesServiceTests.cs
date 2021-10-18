using ET.WebAPI.BusinessLogic.DomainServices;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using NUnit.Framework;
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
        public async Task StoreDeviceAsyncReturnsOperationFailureIfDeviceIsNull()
        { 
            var service = CreateService();

            var result = await service.StoreDeviceAsync(null);

            result.Should().Be(OperationResult.Failure($"device | {OperationErrorMessages.ObjectInvalid}", ErrorType.BusinessLogic));

        }

        private DevicesService CreateService() => new(repository);
    }
}