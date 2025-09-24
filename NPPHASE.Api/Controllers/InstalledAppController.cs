using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using System.ComponentModel.DataAnnotations;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstalledAppController : BaseController<InstalledApp>
    {
        private readonly IInstalledAppService _installedAppService;
        public InstalledAppController(IService<InstalledApp> service, IInstalledAppService installedAppService,
            IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _installedAppService = installedAppService;
        }
        public override async Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _installedAppService.GetAll(model), IsSuccess = true });
        }
        public override async Task<IActionResult> Post([FromBody, Required] InstalledApp entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return await base.Post(entity);
        }
        public override async Task<IActionResult> AddRangeAsync(List<InstalledApp> entities)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entities.ForEach(x =>
            {
                x.DeviceUserId = Convert.ToInt32(deviceData.Data);
            });
            return await base.AddRangeAsync(entities);
        }
        public override IActionResult UpdateRange(List<InstalledApp> entities)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entities.ForEach(x =>
            {
                x.DeviceUserId = Convert.ToInt32(deviceData.Data);
            });
            return base.UpdateRange(entities);
        }

        [HttpPost("RemoveInstalledApp")]
        public async Task<IActionResult> RemoveInstalledApp()
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            return new OkObjectResult(new ResponseMessageViewModel { IsSuccess = await _installedAppService.RemoveInstalledApp(Convert.ToInt32(deviceData.Data)) });
        }
    }
}
