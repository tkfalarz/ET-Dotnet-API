using System;
using System.Runtime.Serialization;

namespace ET.WebAPI.Api.Views
{
    [DataContract]
    public class ReadingView
    {
        [DataMember]
        public DateTimeOffset Timestamp { get; init; }
        
        [DataMember]
        public double Value { get; set; }
    }
}