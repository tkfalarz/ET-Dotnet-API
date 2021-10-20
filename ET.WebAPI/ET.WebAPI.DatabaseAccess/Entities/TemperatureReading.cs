using System;

namespace ET.WebAPI.DatabaseAccess.Entities
{
    public record TemperatureReading
    {
        public DateTimeOffset Timestamp { get; init; }
        public double Value { get; init; }
        public Guid DeviceId { get; init; }
        public Device Device { get; init; }
    }
}