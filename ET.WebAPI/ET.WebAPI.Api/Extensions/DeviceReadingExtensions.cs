using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using System;

namespace ET.WebAPI.Api.Extensions
{
    public static class DeviceReadingExtensions
    {
        public static DeviceReading ToModel(this DeviceReadingView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            return new DeviceReading
            {
                Humidity = view.Humidity,
                Pressure = view.Pressure,
                Temperature = view.Temperature,
                Timestamp = view.Timestamp,
                AirQualityIndex = view.AirQualityIndex,
                DeviceName = view.DeviceName
            };
        }
    }
}