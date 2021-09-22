using ET.WebAPI.DatabaseAccess.DatabaseSetup;
using ET.WebAPI.DatabaseAccess.Entities;
using ET.WebAPI.Kernel;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.DomainServices;
using System.Threading.Tasks;
using GoogleMaps.LocationServices;
using RandomFriendlyNameGenerator;
using System;
using System.Linq;

namespace ET.WebApi.BusinessLogic.DomainServices
{
    public class DeviceService : IDeviceService
    {
        private readonly ApiDbContext dbContext;

        public DeviceService(ApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<OperationResult> StoreDeviceAsync(DeviceModel device)
        {
            if (device.Equals(default)) throw new ArgumentNullException(nameof(device));
            
            var searchingResult = dbContext.Devices.Where(x => x.Id.Equals(device.Id));
            if (searchingResult.Any()) 
                return OperationResult.Failure("Device already exists");

            await dbContext.Devices.AddAsync(new Device
            {
                Id = device.Id,
                Name = GetFriendlyNameForDevice(),
                Latitude = device.Latitude,
                Longitude = device.Longitude,
                SensorName = device.SensorName
            });
            
            await dbContext.SaveChangesAsync();
            return OperationResult.Proceeded();
        }

        private static string GetFriendlyNameForDevice()
        {
            return NameGenerator.Identifiers.Get(
                1,
                IdentifierComponents.FirstName | IdentifierComponents.Adjective | IdentifierComponents.Profession,
                NameOrderingStyle.BobTheBuilderStyle).First();
        }
    }
}