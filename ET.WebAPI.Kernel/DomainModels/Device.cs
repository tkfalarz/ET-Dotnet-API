using System;

namespace ET.WebAPI.Kernel.DomainModels
{
    public record Device
    {
        public Guid DeviceId { get; set; }
        public string DeviceName { get; init; }
        public string SensorName { get; init; }
        public decimal Latitude { get; init; }
        public decimal Longitude { get; init; }
    }
}