using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using System;

namespace ET.WebAPI.Api.Extensions
{
    public static class ReadingExtensions
    {
        public static ReadingView ToView(this Reading reading)
        {
            if (reading is null) throw new ArgumentNullException(nameof(reading));

            return new ReadingView
            {
                Timestamp = reading.Timestamp,
                Value = reading.Value
            };
        }
    }
}