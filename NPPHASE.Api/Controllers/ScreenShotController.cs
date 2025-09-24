using Microsoft.AspNetCore.Mvc;
using NPPHASE.Apis.Controllers;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenShotController : BaseController<ScreenShot>
    {
        private readonly IScreenShotService _screenShotService;

        public ScreenShotController(IService<ScreenShot> service, IScreenShotService screenShotService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _screenShotService = screenShotService;
        }
        public async override Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _screenShotService.GetAll(model), IsSuccess = true });
        }
        [HttpPost("AddScreenShot")]
        public async Task<IActionResult> AddScreenShot(IFormFile files)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            var result = await _screenShotService.AddScreenShot(files, Convert.ToInt32(deviceData.Data));
            return new OkObjectResult(new ResponseMessageViewModel { Data = result, IsSuccess = true });
        }
    }
}
