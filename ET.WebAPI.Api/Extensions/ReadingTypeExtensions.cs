using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using System.ComponentModel;

namespace ET.WebAPI.Api.Extensions
{
    public static class ReadingTypeExtensions
    {
        public static ReadingType ToModel(this ReadingTypeView view)
        {
            return view switch
            {
                ReadingTypeView.Aqi => ReadingType.Aqi,
                ReadingTypeView.Humidity => ReadingType.Humidity,
                ReadingTypeView.Pressure => ReadingType.Pressure,
                ReadingTypeView.Temperature => ReadingType.Temperature,
                _ => throw new InvalidEnumArgumentException("The argument is invalid")
            };
        }
    }
}