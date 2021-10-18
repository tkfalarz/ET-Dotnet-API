using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ET.WebAPI.Kernel.DomainModels
{
    public record Device
    {
        public string SensorName { get; init; }
        public string Latitude { get; init; }
        public string Longitude { get; init; }
        public bool IsValid => SensorName != null && Latitude != null && Longitude != null;
    }
}