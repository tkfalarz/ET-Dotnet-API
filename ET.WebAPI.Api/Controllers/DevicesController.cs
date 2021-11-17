using ET.WebAPI.Api.Extensions;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Kernel.DomainServices;
using ET.WebAPI.Kernel.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
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
        private readonly IDevicesService devicesService;
        private readonly IDevicesRepository devicesRepository;
        private readonly ILogger<DevicesController> logger;

        public DevicesController(IDevicesService devicesService, IDevicesRepository devicesRepository, ILogger<DevicesController> logger)
        {
            this.devicesService = devicesService;
            this.devicesRepository = devicesRepository;
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
            var devices = devicesRepository.GetDevices();

            return devices.Any()
                ? Ok(await devices.Select(x => x.ToView()).ToArrayAsync())
                : NotFound();
        }
    }
}