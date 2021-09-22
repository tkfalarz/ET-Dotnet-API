using System;

namespace ET.WebAPI.Kernel.DomainModels
{
    public record DeviceModel
    {
        public Guid Id { get; init; }
        public string SensorName { get; init; }
        public string Latitude { get; init; }
        public string Longitude { get; init; }
    }
}