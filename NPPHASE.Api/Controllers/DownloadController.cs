using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using System.ComponentModel.DataAnnotations;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : BaseController<Download>
    {
        private readonly IDownloadService _downloadService;
        private readonly IExceptionLoggerServices _exceptionLoggerServices;

        public DownloadController(IService<Download> service, IDownloadService downloadService, IExceptionLoggerServices exceptionLoggerServices, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _downloadService = downloadService;
            _exceptionLoggerServices = exceptionLoggerServices;
        }
        [HttpPost("CreateUpdate")]        
        public async Task<IActionResult> CreateUpdate([Required] bool isAndroid)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int result = await _downloadService.CreateUpdate(isAndroid);
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
                            Message = "Error occured while creating or updating download details"
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
                    ScreenName = "Download/CreateUpdate",
                };
                _exceptionLoggerServices.InsertErrorLog(exception);
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Error occured while creating or updating download details"
                });
            }
        }
        [HttpGet("GetAllDownloadYearAndMonthWise")]       
        public async Task<IActionResult> GetAllDownloadYearAndMonthWise(int year)
        {
            var result = await _downloadService.GetAllDownloadYearAndMonthWise(year);
            return new OkObjectResult(new ResponseMessageViewModel { Data = result, IsSuccess = true });
        }
    }
}
