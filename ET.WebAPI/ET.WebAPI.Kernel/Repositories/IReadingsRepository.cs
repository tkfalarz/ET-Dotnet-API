using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ET.WebAPI.Kernel.Repositories
{
    public interface IReadingsRepository
    {
        Task StoreWeatherFactorsAsync(WeatherReading weatherReading, Guid deviceId);
    }
}