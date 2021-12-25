using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using System;

namespace ET.WebAPI.Api.Extensions
{
    public static class ReadingViewExtensions
    {
        public static ReadingSet ToModel(this ReadingSetView setView)
        {
            if (setView == null)
                throw new ArgumentNullException(nameof(setView));

            return new ReadingSet
            {
                Humidity = setView.Humidity,
                Pressure = setView.Pressure,
                Temperature = setView.Temperature,
                Timestamp = setView.Timestamp,
                AirQualityIndex = setView.AirQualityIndex,
                DeviceName = setView.DeviceName
            };
        }
    }
}