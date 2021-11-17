using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ET.WebAPI.Api.Views
{
    [DataContract]
    public class DeviceView
    {
        [DataMember, Required]
        public string DeviceName { get; set; }

        [DataMember, Required]
        public string SensorName { get; init; }

        [DataMember, Required]
        public decimal Latitude { get; init; }

        [DataMember, Required]
        public decimal Longitude { get; init; }
    }
}