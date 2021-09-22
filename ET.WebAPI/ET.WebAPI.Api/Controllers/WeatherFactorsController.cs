using ET.WebAPI.DatabaseAccess.DatabaseSetup;
using ET.WebAPI.DatabaseAccess.Entities;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ET.WebAPI.Api.Controllers
{
    [ApiController]
    [Route("WeatherFactors")]
    public class WeatherFactorsController : ControllerBase
    {
        private readonly IWeatherReadingService readingService;
        private readonly ILogger<WeatherFactorsController> logger;

        public WeatherFactorsController(IWeatherReadingService readingService, ILogger<WeatherFactorsController> logger)
        {
            this.readingService = readingService ?? throw new ArgumentNullException(nameof(readingService));
        }

        [HttpPost]
        [Authorize]
        [Route("Store")]
        public async Task<IActionResult> StoreFactorsAsync([FromBody, Required] WeatherReadingModel reading)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors).ToList();
                errors.ForEach(error => logger.LogError(error.ErrorMessage));
                return BadRequest();
            }

            var result = await readingService.StoreWeatherReadingAsync(reading);
            if (!result.IsProceeded)
            {
                logger.LogError(result.ErrorMessage);
                return BadRequest();
            }

            return Accepted();
        }
    }
}