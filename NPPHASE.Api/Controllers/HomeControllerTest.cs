using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class HomeControllerTest : ControllerBase
    {
        private readonly IActionResult _result;

        private readonly IHomeService _deviceService;
        private readonly IExceptionLoggerServices _exceptionLoggerServices;
        public HomeControllerTest(IHomeService deviceService, IExceptionLoggerServices exceptionLoggerServices)
        {
            _deviceService = deviceService;
            _exceptionLoggerServices = exceptionLoggerServices;
        }
        [HttpGet("GetAllDeviceUser")]
        [Authorize]
        public async Task<IActionResult> GetAllDeviceUser()
        {
            try
            {
                return new OkObjectResult(new ResponseMessageViewModel()
                {
                    Data = await _deviceService.GetAllDeviceUser(),
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                var exception = new Exceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Timestamp = DateTimeOffset.UtcNow,
                    ScreenName = "Home/GetAllDeviceUser",
                };
                _exceptionLoggerServices.InsertErrorLog(exception);
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    Message = "Error occured while getting details",
                    IsSuccess = false
                });

            }
        }

        [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok("s");
        }


    }
}
