using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using System;

namespace ET.WebAPI.Api.Extensions
{
    public static class DeviceExtensions
    {
        public static Device ToModel(this DeviceView view)
        {
            if (view == default)
                throw new ArgumentNullException(nameof(view), "View cannot be null");

            return new Device
            {
                SensorName = view.SensorName,
                Latitude = view.Latitude,
                Longitude = view.Longitude
            };
        }
    }
}