using ET.WebAPI.Kernel.DomainModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ET.WebAPI.Kernel.Repositories
{
    public interface IReadingsRepository
    {
        Task StoreWeatherFactorsAsync(Reading reading, Guid deviceId);
        IQueryable<Reading> GetDeviceReadings();
    }
}