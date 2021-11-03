using ET.WebAPI.Api.Extensions;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Database.Entities;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> StoreReadingAsync([FromBody, Required] DeviceReadingView reading)
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
    }
}