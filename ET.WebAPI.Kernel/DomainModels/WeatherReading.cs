using System;

namespace ET.WebAPI.Kernel.DomainModels
{
    public class WeatherReading
    {
        public DateTimeOffset Timestamp { get; init; }
        public double AirQualityIndex { get; init; }
        public double Pressure { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
        
            var mapped = (WeatherReading)obj;
            return Timestamp.Equals(mapped.Timestamp)
                   && AirQualityIndex.Equals(mapped.AirQualityIndex)
                   && Pressure.Equals(mapped.Pressure)
                   && Temperature.Equals(mapped.Temperature)
                   && Humidity.Equals(mapped.Humidity);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(Timestamp, AirQualityIndex, Pressure, Temperature, Humidity);
        }
    }
}