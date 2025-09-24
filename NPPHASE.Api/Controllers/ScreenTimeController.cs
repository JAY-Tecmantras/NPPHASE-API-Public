using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ScreenTimeController : BaseController<ScreenTime>
    {
        private readonly IScreenTimeService _screenTimeService;
        private readonly IExceptionLoggerServices _exceptionLoggerServices;

        public ScreenTimeController(IService<ScreenTime> service, IScreenTimeService screenTimeService,
            IExceptionLoggerServices exceptionLoggerServices, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _screenTimeService = screenTimeService;
            _exceptionLoggerServices = exceptionLoggerServices;
        }
        [HttpPost("CreateUpdateScreenTime")]
        public async Task<IActionResult> CreateUpdateScreenTime(ScreenTimeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ResponseMessageViewModel deviceData = GetDeviceId();
                    if (deviceData.IsSuccess == false)
                    {
                        return new OkObjectResult(deviceData);
                    }
                    model.DeviceUserId = Convert.ToInt32(deviceData.Data);

                    int result = await _screenTimeService.CreateUpdateScreenTime(model);
                    if (result == 1)
                    {
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            IsSuccess = true
                        });
                    }
                    else
                    {
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            IsSuccess = false,
                            Message = "Error occured while creating or updating screen time"
                        });
                    }
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "Model is not validate"
                    });
                }
            }
            catch (Exception ex)
            {
                var exception = new Exceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Timestamp = DateTimeOffset.UtcNow,
                    ScreenName = "ScreenTime/CreateUpdateScreenTime",
                };
                _exceptionLoggerServices.InsertErrorLog(exception);
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Error occured while creating or updating screen time"
                });
            }

        }
        public override async Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _screenTimeService.GetAll(model), IsSuccess = true });
        }
        public override async Task<IActionResult> AddRangeAsync(List<ScreenTime> entities)
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
        [HttpGet("Top5AppAndScreenTime")]       
        public async Task<IActionResult> Top5AppAndScreenTime([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _screenTimeService.Top5AppAndScreenTime(model), IsSuccess = true });
        }
    }
}
