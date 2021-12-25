using ET.WebAPI.Api.Extensions;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace ET.WebAPI.Api.Tests.Unit.Extensions
{
    [TestFixture]
    public class ReadingSetViewExtensionsTests
    {
        [Test]
        public void ThrowsArgumentNullExceptionIfViewIsNull()
        {
            Action action = () => ((ReadingSetView)null).ToModel();

            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("setView");
        }

        [Test]
        public void ToModelTest()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            var view = new ReadingSetView
            {
                Humidity = 2,
                Pressure = 1,
                Temperature = 3,
                AirQualityIndex = 7,
                Timestamp = dateTimeOffset,
                DeviceName = "Dev1"
            };
            var expectedModel = new ReadingSet
            {
                Humidity = 2,
                Pressure = 1,
                Temperature = 3,
                AirQualityIndex = 7,
                Timestamp = dateTimeOffset,
                DeviceName = "Dev1"
            };
            var result = view.ToModel();

            result.Should().Be(expectedModel);
        }
    }
}