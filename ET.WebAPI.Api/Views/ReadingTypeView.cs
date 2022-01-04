using System.Runtime.Serialization;

namespace ET.WebAPI.Api.Views
{
    [DataContract]
    public enum ReadingTypeView
    {
        [EnumMember]
        Pressure,
        [EnumMember]
        Humidity,
        [EnumMember]
        Temperature,
        [EnumMember]
        Aqi
    }
}