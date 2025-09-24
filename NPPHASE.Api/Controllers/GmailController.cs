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

    public class GmailController : BaseController<Gmail>
    {
        private readonly IGmailService _gmailService;
        public GmailController(IService<Gmail> service, IGmailService gmailService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _gmailService = gmailService;
        }
        [HttpGet("GetAllGmails")]        
        public async Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model, string? email, string? message)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _gmailService.GetAll(model, email, message), IsSuccess = true });

        }
        public override async Task<IActionResult> Post([FromBody, Required] Gmail entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return await base.Post(entity);
        }
        public override async Task<IActionResult> AddRangeAsync(List<Gmail> entities)
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
