using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using System;

namespace ET.WebAPI.Api.Extensions
{
    public static class ReadingSetExtensions
    {
        public static ReadingSetView ToView(this ReadingSet model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return new ReadingSetView
            {
                Humidity = model.Humidity,
                Pressure = model.Pressure,
                Temperature = model.Temperature,
                Timestamp = model.Timestamp,
                DeviceName = model.DeviceName,
                AirQualityIndex = model.AirQualityIndex
            };
        }
    }
}