using ET.WebAPI.DatabaseAccess.DatabaseSetup;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ET.WebAPI.DatabaseAccess.Repositories
{
    public class DevicesRepository : IDevicesRepository
    {
        private readonly ApiDbContext dbContext;
        private readonly ILogger<DevicesRepository> logger;
        public DevicesRepository(ApiDbContext dbContext, ILogger<DevicesRepository> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<OperationResult<Guid>> GetDeviceIdAsync(string deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
                throw new ArgumentNullException(nameof(deviceName), "Device name cannot be null or empty");

            var result = await dbContext.Devices.FirstOrDefaultAsync(x => x.Name == deviceName);
            if (result == default)
            {
                return OperationResult<Guid>.Failure($"Device {deviceName} not found", ErrorType.Entity);

            }

            return OperationResult<Guid>.Proceeded(result.Id);
        }
        
        public async Task StoreDeviceAsync(Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device), "Device cannot be null");

            await dbContext.Devices.AddAsync(
                new Entities.Device
                {
                    Latitude = device.Latitude,
                    Longitude = device.Longitude,
                    Name = device.SensorName
                });

            await dbContext.SaveChangesAsync();
        }
    }
}