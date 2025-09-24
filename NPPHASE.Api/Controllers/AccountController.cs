using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IDeviceUserServices _deviceUserServices;
        private readonly IExceptionLoggerServices _exceptionLoggerServices;
        public AccountController(UserManager<User> userManager, IDeviceUserServices deviceUserServices, IExceptionLoggerServices exceptionLoggerServices)
        {
            _userManager = userManager;
            _deviceUserServices = deviceUserServices;
            _exceptionLoggerServices = exceptionLoggerServices;
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string imeiNumber = $"{model.IMEINumber}@gmail.com";
                    var Modeluser = await _userManager.FindByEmailAsync(imeiNumber);
                    if (Modeluser == null)
                    {
                        var user = new User
                        {
                            UserName = model.UserName,
                            Email = $"{model.IMEINumber}@gmail.com",
                            EmailConfirmed = true,
                            PhoneNumberConfirmed = true,
                            CreationDate = DateTimeOffset.UtcNow,
                            PasswordNormal = model.IMEINumber
                        };
                        var result = await _userManager.CreateAsync(user, model.IMEINumber);
                        if (result.Succeeded)
                        {
                            var roles = await _userManager.AddToRoleAsync(user, "User");
                            if (roles.Succeeded)
                            {
                                return new OkObjectResult(new ResponseMessageViewModel
                                {
                                    IsSuccess = true,
                                    Message = "User added successfully"
                                });
                            }
                            else
                            {
                                return new OkObjectResult(new ResponseMessageViewModel
                                {
                                    IsSuccess = false,
                                    Message = roles.Errors.FirstOrDefault().Description
                                });
                            }
                        }
                        else
                        {
                            return new OkObjectResult(new ResponseMessageViewModel
                            {
                                IsSuccess = false,
                                Message = result.Errors.FirstOrDefault().Description
                            });
                        }

                    }
                    else
                    {

                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            Message = "Login successfully",
                            IsSuccess = true
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
                    ScreenName = "Account/Login",
                };
                _exceptionLoggerServices.InsertErrorLog(exception);
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Error occured"
                });
            }

        }
        
        [HttpPost("UpdateDeviceToken")]
        public async Task<IActionResult> UpdateDeviceToken(UpdateDeviceTokenViewModel viewModel)
        {
            try
            {
                Request.Headers.TryGetValue("IMEINumber", out StringValues imeiNumber);
                var deviceId = _deviceUserServices.GetDeviceUserIdByImemiNumber(imeiNumber).ToString();

                if (deviceId == null || string.IsNullOrEmpty(deviceId))
                {
                    return new OkObjectResult(new ResponseMessageViewModel { Message = "Please add device User first.", IsSuccess = false });
                }
                viewModel.DeviceUserId = Convert.ToInt32(deviceId);
                return new OkObjectResult(await _deviceUserServices.UpdateDeviceToken(viewModel));

            }
            catch (Exception ex)
            {
                var exception = new Exceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Timestamp = DateTimeOffset.UtcNow,
                    ScreenName = "Account/UpdateDeviceToken",
                };
                _exceptionLoggerServices.InsertErrorLog(exception);
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Error occured while updating device token"
                });
            }
        }
        [NonAction]
        public ResponseMessageViewModel GetDeviceId()
        {
            Request.Headers.TryGetValue("IMEINumber", out StringValues imeiNumber);
            int? DeviceId = _deviceUserServices.GetDeviceUserIdByImemiNumber(imeiNumber);
            if (string.IsNullOrEmpty(imeiNumber))
            {
                return new ResponseMessageViewModel { Message = "Please add header imeinumber first.", IsSuccess = false };
            }
            else if (DeviceId.HasValue)
            {
                return new ResponseMessageViewModel { Message = "Please add device User first.", IsSuccess = false };

            }
            else
            {
                return new ResponseMessageViewModel { Data = DeviceId, IsSuccess = true };

            }
        }
    }
}
