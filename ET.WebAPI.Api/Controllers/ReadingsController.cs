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
        public async Task<IActionResult> StoreReadingAsync([FromBody, Required] ReadingSetView readingSet)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors).ToList();
                errors.ForEach(error => logger.LogError(error.ErrorMessage));
                return BadRequest();
            }

            var result = await readingsService.StoreReadingSetAsync(readingSet.ToModel());
            if (result.IsProceeded) return Accepted();
            logger.LogError(result.ErrorMessage);
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [Route("nearest")]
        public async Task<IActionResult> GetNearestLatestReadingsSetAsync([FromQuery] decimal latitude, [FromQuery] decimal longitude)
        {
            var readings = await readingsService.GetNearestLatestReadingsAsync(latitude, longitude);
            return readings != null
                ? Ok(readings.ToView())
                : NotFound();
        }
    }
}