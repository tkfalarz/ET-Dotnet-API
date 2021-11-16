using ET.WebAPI.Api.Extensions;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ET.WebAPI.Api.Controllers
{
    [ApiController]
    [Route("api/Readings")]
    public class ReadingsController : ControllerBase
    {
        private readonly IReadingsService readingsService;
        private readonly ILogger<ReadingsController> logger;

        public ReadingsController(IReadingsService readingsService, ILogger<ReadingsController> logger)
        {
            this.readingsService = readingsService;
            this.logger = logger;
        }

        [HttpPost]
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.Accepted)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> StoreReadingAsync([FromBody, Required] ReadingView reading)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors).ToList();
                errors.ForEach(error => logger.LogError(error.ErrorMessage));
                return BadRequest();
            }

            var result = await readingsService.StoreWeatherReadingAsync(reading.ToModel());
            if (!result.IsProceeded)
            {
                logger.LogError(result.ErrorMessage);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            return Accepted();
        }
        
        [HttpGet]
        [Route("{deviceName}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ReadingView[]))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetDeviceReadingsAsync([Required] string deviceName)
        {
            var result = await readingsService.GetDeviceReadingsAsync(deviceName);
            return result.Any()
                ? Ok(result.Select(x => x.ToView()).ToArray())
                : NotFound();
        }

        [HttpGet]
        [Route("latest")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ReadingView))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetNearestLatestReadingAsync([FromQuery, Required] decimal latitude, decimal longitude)
        {
            var result = await readingsService.GetNearestLatestReadingAsync(latitude, longitude);
            return result == null
                ? NotFound()
                : Ok(result.ToView());
        }

        [HttpGet]
        [Route("latest/allDevices")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ReadingView[]))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetDevicesLatestReadingsAsync()
        {
            var result = await readingsService.GetLatestReadingsAsync();
            return result.Any()
                ? Ok(result.Select(x=>x.ToView()).ToArray())
                : NotFound();
        }
    }
}