using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using ET.WebAPI.Kernel.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ET.WebAPI.Database.Repositories
{
    public class DevicesRepository : IDevicesRepository
    {
        private readonly ApiDbContext dbContext;
        public DevicesRepository(ApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<OperationResult<Guid>> GetDeviceIdAsync(string deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
                throw new ArgumentNullException(nameof(deviceName), "Device name cannot be null or empty");

            var result = await dbContext.Devices.FirstOrDefaultAsync(x => x.Name == deviceName);
            if (result == null)
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
                    Name = device.DeviceName,
                    SensorName = device.SensorName
                });

            await dbContext.SaveChangesAsync();
        }
    }
}