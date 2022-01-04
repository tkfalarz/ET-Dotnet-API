using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ET.WebAPI.Kernel.DomainServices
{
    public interface IDevicesService
    {
        Task<OperationResult> StoreDeviceAsync(Device device);
        Task<IReadOnlyList<Device>> GetDevicesAsync();
        Task<Device> GetDeviceAsync(string deviceName);
    }
}