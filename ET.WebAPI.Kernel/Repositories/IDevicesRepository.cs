using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using System;
using System.Threading.Tasks;

namespace ET.WebAPI.Kernel.Repositories
{
    public interface IDevicesRepository
    {
        Task<OperationResult<Guid>> GetDeviceIdAsync(string deviceName);
        Task StoreDeviceAsync(Device device);
    }
}