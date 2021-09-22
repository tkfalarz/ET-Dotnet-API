using ET.WebAPI.Kernel.DomainModels;
using System.Threading.Tasks;

namespace ET.WebAPI.Kernel.DomainServices
{
    public interface IWeatherReadingService
    {
        Task<OperationResult> StoreWeatherReadingAsync(WeatherReadingModel weatherReading);
    }
}