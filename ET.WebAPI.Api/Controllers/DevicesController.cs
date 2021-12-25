using ET.WebAPI.Api.Extensions;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ET.WebAPI.Api.Controllers
{
    [ApiController]
    [Route("api/Devices")]
    public class DevicesController : ControllerBase
    {
        private const string LimitCriteriaBelowValueErrorMessage = "Limit criteria below 0 value";
        private const string FactorNotSupportedErrorMessage = "This type of weather factor is not supported";
        private readonly IDevicesService devicesService;
        private readonly IReadingsService readingsService;
        private readonly ILogger<DevicesController> logger;

        public DevicesController(IDevicesService devicesService, IReadingsService readingsService, ILogger<DevicesController> logger)
        {
            this.devicesService = devicesService;
            this.readingsService = readingsService;
            this.logger = logger;
        }

        [HttpPost]
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.Accepted)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> StoreDeviceAsync([FromBody, Required] DeviceView reading)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors).ToList();
                errors.ForEach(error => logger.LogError(error.ErrorMessage));
                return BadRequest();
            }

            var result = await devicesService.StoreDeviceAsync(reading.ToModel());
            if (result.IsFailure)
            {
                logger.LogError(result.ErrorMessage);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            return Accepted();
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(DeviceView[]))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetDevicesAsync()
        {
            var devices = await devicesService.GetDevicesAsync();

            return devices.Any()
                ? Ok(devices.Select(x => x.ToView()).ToList())
                : NotFound();
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(DeviceView))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [Route("{deviceName}")]
        public async Task<IActionResult> GetDeviceAsync(string deviceName)
        {
            var device = await devicesService.GetDeviceAsync(deviceName);
            return device != null
                ? Ok(device.ToView())
                : NotFound();
        }

        [HttpGet]
        [Route("{deviceName}/Readings")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ReadingSetView[]))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetDeviceReadingsAsync([Required] string deviceName, [FromQuery] int limit)
        {
            if (limit < 0)
                return BadRequest(LimitCriteriaBelowValueErrorMessage);

            var result = await readingsService.GetReadingsAsync(deviceName, limit);
            return result != null && result.Any()
                ? Ok(result.Select(x => x.ToView()).ToList())
                : NotFound();
        }

        [HttpGet]
        [Route("{deviceName}/Readings/Latest")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ReadingSetView))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetDeviceLatestReadingsAsync([Required] string deviceName)
        {
            var result = await readingsService.GetLatestReadingsAsync(deviceName);
            return result != null
                ? Ok(result.ToView())
                : NotFound();
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ReadingView[]))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [Route("{deviceName}/Readings/{readingType}")]
        public async Task<IActionResult> GetDeviceWeatherFactorReadingsAsync(string deviceName, ReadingType readingType, [FromQuery] int limit)
        {
            if (!Enum.IsDefined(typeof(ReadingType), readingType))
                return BadRequest(FactorNotSupportedErrorMessage);
            if (limit < 0)
                return BadRequest(LimitCriteriaBelowValueErrorMessage);

            var result = await readingsService.GetTypedReadingsAsync(deviceName, readingType, limit);
            return result != null && result.Any()
                ? Ok(result.Select(x => x.ToView()).ToList())
                : NotFound();
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ReadingView))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [Route("{deviceName}/Readings/{readingType}/Latest")]
        public async Task<IActionResult> GetDeviceWeatherFactorLatestReadingAsync(string deviceName, ReadingType readingType)
        {
            if (!Enum.IsDefined(typeof(ReadingType), readingType))
                return BadRequest(FactorNotSupportedErrorMessage);

            var result = await readingsService.GetTypedLatestReadingAsync(deviceName, readingType);
            return result != null
                ? Ok(result.ToView())
                : NotFound();
        }
    }
}