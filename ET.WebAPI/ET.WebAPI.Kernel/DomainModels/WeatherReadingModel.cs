using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ET.WebAPI.Kernel.DomainModels
{
    [DataContract]
    public record WeatherReadingModel
    {
        [DataMember, Required]
        public DateTimeOffset Timestamp { get; init; }
        [DataMember, Required]
        public string DeviceName { get; set; }
        [DataMember, Required]
        public double AirQualityIndex { get; init; }
        [DataMember, Required]
        public double Pressure { get; set; }
        [DataMember, Required]
        public double Temperature { get; set; }
        [DataMember, Required]
        public double Humidity { get; set; }
    }
}