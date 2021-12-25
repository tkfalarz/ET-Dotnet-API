using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.DomainServices;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ET.WebAPI.BusinessLogic.Services
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
            if (device == null) throw new ArgumentNullException(nameof(device));

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

        public async Task<IReadOnlyList<Device>> GetDevicesAsync()
        {
            var devices = await devicesRepository.GetDevices().ToListAsync();
            return devices;
        }

        public async Task<Device> GetDeviceAsync(string deviceName)
        {
            var device = await devicesRepository.GetDevices()
                .Where(x => x.DeviceName == deviceName)
                .Take(1)
                .FirstOrDefaultAsync();
            return device;
        }
    }
}