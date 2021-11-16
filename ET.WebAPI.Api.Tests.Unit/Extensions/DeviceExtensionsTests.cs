using ET.WebAPI.Api.Extensions;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace ET.WebAPI.Api.Tests.Unit.Extensions
{
    [TestFixture]
    public class DeviceExtensionsTests
    {
        [Test]
        public void ThrowsArgumentNullExceptionIfDeviceIsNull()
        {
            Action action = () => ((DeviceView)null).ToModel();

            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("view");
        }
        
        [Test]
        public void ToModelTest()
        {
            var view = new DeviceView
            {
                DeviceName = "Device1",
                SensorName = "Sensor1",
                Latitude = 11.1111m,
                Longitude = 22.2222m
            };
            var expectedResult = new Device
            {
                DeviceName = "Device1",
                SensorName = "Sensor1",
                Latitude = 11.1111m,
                Longitude = 22.2222m
            };

            var result = view.ToModel();

            result.Should().Be(expectedResult);
        }
    }
}