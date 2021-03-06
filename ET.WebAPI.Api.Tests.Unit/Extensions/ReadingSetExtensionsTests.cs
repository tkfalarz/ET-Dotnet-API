using ET.WebAPI.Api.Extensions;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace ET.WebAPI.Api.Tests.Unit.Extensions
{
    [TestFixture]
    public class ReadingSetExtensionsTests
    {
        [Test]
        public void ToViewShouldThrowArgumentNullExceptionIfModelIsNull()
        {
            Action action = () => ((ReadingSet)null).ToView();

            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("model");
        }

        [Test]
        public void ToViewTests()
        {
            const double aqi = 232;
            const double hum = 123;
            const double temp = 11;
            const double press = 999;
            var dto = DateTimeOffset.Now;
            var model = new ReadingSet
            {
                Humidity = hum,
                Pressure = press,
                Temperature = temp,
                Timestamp = dto,
                DeviceName = "dev1",
                AirQualityIndex = aqi
            };
            var expectedResult = new ReadingSetView
            {
                Humidity = hum,
                Pressure = press,
                Temperature = temp,
                Timestamp = dto,
                DeviceName = "dev1",
                AirQualityIndex = aqi
            };

            var result = model.ToView();

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}