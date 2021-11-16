using ET.WebAPI.Api.Extensions;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace ET.WebAPI.Api.Tests.Unit.Extensions
{
    [TestFixture]
    public class DeviceViewExtensionsTests
    {
        [Test]
        public void ToViewShouldThrowArgumentNullExceptionIfModelIsNull()
        {
            Action action = () => ((Device)null).ToView();

            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("model");
        }

        [Test]
        public void ToViewTests()
        {
            const decimal lat = 221m;
            const decimal lon = 250m;
            var model = new Device
            {
                Latitude = lat,
                Longitude = lon,
                DeviceName = "Dev1",
                SensorName = "Sen1"
            };
            var expectedResult = new DeviceView
            {
                Latitude = lat,
                Longitude = lon,
                DeviceName = "Dev1",
                SensorName = "Sen1"
            };

            var result = model.ToView();

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}