using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.DomainServices;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ET.WebAPI.BusinessLogic.DomainServices
{
    public class DevicesService : IDevicesService
    {
        private readonly IDevicesRepository devicesRepository;

        public DevicesService(IDevicesRepository devicesRepository)
        {
            this.devicesRepository = devicesRepository;
        }

        public async Task<OperationResult> StoreDeviceAsync(Device device)
        {
            if (device == default || !device.IsValid)
                return OperationResult.Failure(
                    $"{nameof(device)} | {OperationErrorMessages.ObjectInvalid}",
                    ErrorType.BusinessLogic);

            try
            {
                await devicesRepository.StoreDeviceAsync(device);
            }
            catch (DbUpdateException exception)
            {
                return OperationResult.Failure(exception.Message, ErrorType.Entity);
            }

            return OperationResult.Proceeded();
        }
    }
}