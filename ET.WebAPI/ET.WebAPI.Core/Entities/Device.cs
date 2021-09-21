using System;
using System.Collections.Generic;

namespace ET.WebAPI.Core.Entities
{
    public record Device
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string SensorName { get; init; }
        public string Latitude { get; init; }
        public string Longitude { get; init; }
        public List<AqiReading> AqiReadings { get; init; }
        public List<HumidityReading> HumidityReadings { get; init; }
        public List<PressureReading> PressureReadings { get; init; }
        public List<TemperatureReading> TemperatureReadings { get; init; }
    }
}