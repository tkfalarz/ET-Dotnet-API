using ET.WebAPI.Kernel.DomainModels;
using System.Linq;
using System.Threading.Tasks;

namespace ET.WebAPI.Kernel.Repositories
{
    public interface IDevicesRepository
    {
        IQueryable<Device> GetDevices();
        Task StoreDeviceAsync(Device device);
    }
}