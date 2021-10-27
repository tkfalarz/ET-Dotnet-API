using System;
using System.Collections.Generic;

namespace ET.WebAPI.DatabaseAccess.Entities
{
    public record Device
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string SensorName { get; init; }
        public string Latitude { get; init; }
        public string Longitude { get; init; }
        public ICollection<AqiReading> AqiReadings { get; init; }
        public ICollection<HumidityReading> HumidityReadings { get; init; }
        public ICollection<PressureReading> PressureReadings { get; init; }
        public ICollection<TemperatureReading> TemperatureReadings { get; init; }
    }
}