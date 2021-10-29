using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using System.Threading.Tasks;

namespace ET.WebAPI.Kernel.DomainServices
{
    public interface IReadingsService
    {
        Task<OperationResult> StoreWeatherReadingAsync(DeviceReading deviceReading);
        
    }
}