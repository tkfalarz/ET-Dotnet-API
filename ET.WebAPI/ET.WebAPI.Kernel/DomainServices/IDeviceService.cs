using ET.WebAPI.Kernel.DomainModels;
using System.Threading.Tasks;

namespace ET.WebAPI.Kernel.DomainServices
{
    public interface IDeviceService
    {
        Task<OperationResult> StoreDeviceAsync(DeviceModel device);
    }
}