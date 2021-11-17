using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using System;

namespace ET.WebAPI.Api.Extensions
{
    public static class DeviceViewExtensions
    {
        public static DeviceView ToView(this Device model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return new DeviceView
            {
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                DeviceName = model.DeviceName,
                SensorName = model.SensorName
            };
        }
    }
}