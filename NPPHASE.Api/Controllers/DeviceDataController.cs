using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Enum;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class DeviceDataController : BaseController<DeviceData>
    {
        private readonly IDeviceDataService _deviceDataService;
        private readonly IExceptionLoggerServices _exceptionLoggerService;
        public DeviceDataController(IService<DeviceData> service, IDeviceDataService deviceDataService, IExceptionLoggerServices exceptionLoggerService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _deviceDataService = deviceDataService;
            _exceptionLoggerService = exceptionLoggerService;
        }
        public async override Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _deviceDataService.GetAll(model), IsSuccess = true });
        }
        [HttpPost("CreateUpdate")]
        public async Task<IActionResult> CreateUpdate(DeviceData device)
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

                    device.DeviceUserId = Convert.ToInt32(deviceData.Data);
                    int result = await _deviceDataService.CreateUpdate(device);
                    if (result == 1)
                    {
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            Message = "Create or update device details successfully",
                            IsSuccess = true
                        });
                    }
                    else
                    {
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            IsSuccess = false,
                            Message = "Error occured while creating or updating device details"
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
                    ScreenName = "DeviceData/CreateUpdate",
                };
                _exceptionLoggerService.InsertErrorLog(exception);
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Error occured while creating or updating device details"
                });
            }
        }
        public override async Task<IActionResult> AddRangeAsync(List<DeviceData> entities)
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
        public override IActionResult UpdateRange(List<DeviceData> entities)
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

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateDeviceStatus request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "Model is not valid"
                    });
                }

                if (!Enum.IsDefined(typeof(DeviceStatus), request.Status.Value))
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = $"Invalid status value: {request.Status.Value}. " +
                                  $"Allowed values are {string.Join(", ", Enum.GetNames(typeof(DeviceStatus)))}"
                    });
                }

                int result = await _deviceDataService.UpdateStatus(request.DeviceUserId, request.Status.Value);

                if (result == 1)
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Message = "Status updated successfully"
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = $"Device User Data with Id {request.DeviceUserId} not found."
                    });
                }
            }
            catch (Exception ex)
            {
                _exceptionLoggerService?.InsertErrorLog(new Exceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Timestamp = DateTimeOffset.UtcNow,
                    ScreenName = "DeviceData/UpdateStatus"
                });

                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Error occurred while updating device status"
                });
            }
        }
    }
}
