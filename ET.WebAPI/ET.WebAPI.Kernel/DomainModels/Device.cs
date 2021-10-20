using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ET.WebAPI.Kernel.DomainModels
{
    public record Device
    {
        public string DeviceName { get; init; }
        public string SensorName { get; init; }
        public string Latitude { get; init; }
        public string Longitude { get; init; }
    }
}