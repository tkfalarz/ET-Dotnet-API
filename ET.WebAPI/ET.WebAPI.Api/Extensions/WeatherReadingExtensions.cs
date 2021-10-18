using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using System;

namespace ET.WebAPI.Api.Extensions
{
    public static class WeatherReadingExtensions
    {
        public static DeviceReading ToModel(this DeviceReadingView view)
        {
            if (view == default)
                throw new ArgumentNullException(nameof(view), "The view is null");

            return new DeviceReading
            {
                Humidity = view.Humidity,
                Pressure = view.Pressure,
                Temperature = view.Pressure,
                Timestamp = view.Timestamp,
                DeviceName = view.DeviceName,
                AirQualityIndex = view.AirQualityIndex
            };
        }
    }
}