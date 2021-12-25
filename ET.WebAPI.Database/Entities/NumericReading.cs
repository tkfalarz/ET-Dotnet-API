using System;

namespace ET.WebAPI.Database.Entities
{
    public record NumericReading
    {
        public NumericReadingType ReadingType { get; init; }
        public DateTimeOffset Timestamp { get; init; }
        public double Value { get; init; }
        public Guid DeviceId { get; init; }
        public Device Device { get; init; }
    }
}