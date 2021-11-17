using System;
using System.Collections.Generic;

namespace ET.WebAPI.Database.Entities
{
    public record Device
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string SensorName { get; init; }
        public decimal Latitude { get; init; }
        public decimal Longitude { get; init; }
        public ICollection<AqiReading> AqiReadings { get; init; }
        public ICollection<HumidityReading> HumidityReadings { get; init; }
        public ICollection<PressureReading> PressureReadings { get; init; }
        public ICollection<TemperatureReading> TemperatureReadings { get; init; }
    }
}