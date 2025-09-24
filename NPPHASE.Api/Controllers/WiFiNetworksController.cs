using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class WiFiNetworksController : BaseController<WiFiNetwork>
    {
        private readonly IWiFiNetworksService _wiFiNetworksService;

        public WiFiNetworksController(IService<WiFiNetwork> service, IWiFiNetworksService wiFiNetworksService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _wiFiNetworksService = wiFiNetworksService;
        }
        public override async Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _wiFiNetworksService.GetAll(model), IsSuccess = true });
        }
        public override async Task<IActionResult> AddRangeAsync(List<WiFiNetwork> entities)
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
        public override IActionResult UpdateRange(List<WiFiNetwork> entities)
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
    }
}
