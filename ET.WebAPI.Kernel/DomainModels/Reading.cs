using System;

namespace ET.WebAPI.Kernel.DomainModels
{
    public class Reading
    {
        public DateTimeOffset Timestamp { get; set; }
        public double Value { get; set; }
    }
}