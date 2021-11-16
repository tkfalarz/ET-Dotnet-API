using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ET.WebAPI.Kernel.DomainServices
{
    public interface IReadingsService
    {
        Task<OperationResult> StoreWeatherReadingAsync(Reading reading);

        Task<Reading> GetNearestLatestReadingAsync(decimal latitude, decimal longitude);
        Task<List<Reading>> GetLatestReadingsAsync();
        Task<Reading[]> GetDeviceReadingsAsync(string deviceName);
    }
}