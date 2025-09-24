using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Common;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class DeviceUserController : BaseController<DeviceUser>
    {
        private readonly IService<User> _userService;
        private readonly IDeviceUserServices _deviceUserServices;
        private readonly IExceptionLoggerServices _exceptionLoggerServices;
        private readonly UserManager<User> _userManager;

        public DeviceUserController(IService<DeviceUser> service, UserManager<User> userManager,
            IService<User> userService, IDeviceUserServices deviceUserServices, IExceptionLoggerServices exceptionLoggerServices) : base(service, deviceUserServices)
        {
            _userService = userService;
            _deviceUserServices = deviceUserServices;
            _exceptionLoggerServices = exceptionLoggerServices;
            _userManager = userManager;
        }
        public override async Task<IActionResult> Post([FromBody] DeviceUser entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entity.DeviceUniqueId = CommonFunctions.GenerateAlphabaticNumber(6);
                    GetAllRequestViewModel getAll = new GetAllRequestViewModel();
                    var userid = (await _userManager.FindByEmailAsync($"{entity.IMEINumber}@gmail.com")).Id;
                    entity.UserId = userid;
                    var user = _userService.GetByIdAsync(entity.UserId).Result;
                    var deviceUser = await base.Post(entity);
                    if (deviceUser != null)
                    {
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            IsSuccess = true,
                            Message = "Device added"
                        });
                    }
                    else
                    {
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            Message = "Error occured while creating device",
                            IsSuccess = false
                        });
                    }
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        Message = "Model is not validate",
                        IsSuccess = false
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
                    ScreenName = "DeviceUser",
                };
                _exceptionLoggerServices.InsertErrorLog(exception);
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    Message = "Error occured while creating device",
                    IsSuccess = false
                });

            }


        }
        public async override Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _deviceUserServices.GetAll(model), IsSuccess = true });
        }
        [HttpGet("GetDeviceDetailByDeviceId")]        
        public async Task<IActionResult> GetDeviceDetailByDeviceId(int deviceUserId)
        {
            try
            {
                var result = await _deviceUserServices.GetDeviceById(deviceUserId);
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                var exception = new Exceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Timestamp = DateTimeOffset.UtcNow,
                    ScreenName = "DeviceUser/GetDeviceDetailByDeviceId",
                };
                _exceptionLoggerServices.InsertErrorLog(exception);
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Something went wrong"
                });
            }
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset(int deviceUserId)
        {

            var result = await _deviceUserServices.ResetAlacProgressColumn(deviceUserId);
            return new OkObjectResult(new ResponseMessageViewModel
            {
                IsSuccess = result,
                Data = result
            });
        }
    }
}
