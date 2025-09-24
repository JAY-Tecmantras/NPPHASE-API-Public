using Microsoft.AspNetCore.Mvc;
using NPPHASE.Apis.Controllers;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurroundRecordingController : BaseController<SurroundRecordings>
    {
        private readonly ISurroundRecordingService _surroundRecordingService;

        public SurroundRecordingController(IService<SurroundRecordings> service, ISurroundRecordingService surroundRecordingService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _surroundRecordingService = surroundRecordingService;
        }
        [HttpPost("AddSurroundRecording")]
        public async Task<IActionResult> AddSurroundRecording([FromForm] SurroundRecordingViewModel surroundRecordingViewModel)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            surroundRecordingViewModel.DeviceUserId = Convert.ToInt32(deviceData.Data);
            var result = await _surroundRecordingService.AddSurroundRecording(surroundRecordingViewModel);
            return new OkObjectResult(new ResponseMessageViewModel { Data = result, IsSuccess = true });
        }
        public async override Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _surroundRecordingService.GetAll(model), IsSuccess = true });
        }
    }
}
