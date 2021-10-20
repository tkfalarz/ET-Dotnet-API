using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using System.Threading.Tasks;

namespace ET.WebAPI.Kernel.DomainServices
{
    public interface IDevicesService
    {
        Task<OperationResult> StoreDeviceAsync(Device device);
    }
}