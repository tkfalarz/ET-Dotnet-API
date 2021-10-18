using ET.WebAPI.Api.Extensions;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ET.WebAPI.Api.Controllers
{
    [ApiController]
    [Route("api/Devices")]
    public class DevicesController : ControllerBase
    {
        private readonly IDevicesService devicesService;
        private readonly ILogger<DevicesController> logger;

        public DevicesController(IDevicesService devicesService, ILogger<DevicesController> logger)
        {
            this.devicesService = devicesService;
            this.logger = logger;
        }

        [HttpPost]
        [Authorize]
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

    }
}