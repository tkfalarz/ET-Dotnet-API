using System;

namespace ET.WebAPI.Core.Entities
{
    public record AqiReading
    {
        public DateTimeOffset Timestamp { get; init; }
        public double Value { get; init; }
        public Guid DeviceId { get; init; }
        public Device Device { get; init; }
    }
}