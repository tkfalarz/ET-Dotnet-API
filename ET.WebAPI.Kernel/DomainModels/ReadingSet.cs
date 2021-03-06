using System;

namespace ET.WebAPI.Kernel.DomainModels
{
    public record ReadingSet
    {
        public string DeviceName { get; set; }
        public DateTimeOffset Timestamp { get; init; }
        public double? AirQualityIndex { get; init; }
        public double? Pressure { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
    }
}