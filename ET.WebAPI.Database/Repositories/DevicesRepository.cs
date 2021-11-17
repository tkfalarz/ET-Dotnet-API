using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.Repositories;
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
        
        public IQueryable<Device> GetDevices()
        {
            var result = dbContext.Devices.Select(
                x => new Device
                {
                    DeviceId = x.Id,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    DeviceName = x.Name,
                    SensorName = x.SensorName
                });

            return result;
        }
    }
}