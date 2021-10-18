using ET.WebAPI.BusinessLogic.DomainServices;
using ET.WebAPI.DatabaseAccess.Repositories;
using ET.WebAPI.Kernel.DomainServices;
using ET.WebAPI.Kernel.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ET.WebAPI.Api
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IReadingsRepository, ReadingsRepository>()
                .AddScoped<IDevicesRepository, DevicesRepository>()
                .AddScoped<IReadingsService, ReadingsService>()
                .AddScoped<IDevicesService, DevicesService>();
        }
    }
}