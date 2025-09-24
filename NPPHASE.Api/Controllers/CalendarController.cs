using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using System.ComponentModel.DataAnnotations;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : BaseController<Calendar>
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(IService<Calendar> service, ICalendarService calendarService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _calendarService = calendarService;
        }
        public override async Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _calendarService.GetAll(model), IsSuccess = true });

        }
        public override async Task<IActionResult> Post([FromBody, Required] Calendar entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return await base.Post(entity);
        }
    }
}
