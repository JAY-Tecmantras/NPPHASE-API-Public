using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using NPPHASE.Services.Repositories;
using System.ComponentModel.DataAnnotations;

namespace NPPHASE.Apis.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]

    public class WhatsappController : BaseController<Whatsapp>
    {
        private readonly IWhatsappService _whatsappService;

        public WhatsappController(IService<Whatsapp> service, IWhatsappService whatsappService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _whatsappService = whatsappService;
        }
        public override async Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _whatsappService.GetAll(model), IsSuccess = true });
        }
        public override async Task<IActionResult> Post([FromBody, Required] Whatsapp entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return await base.Post(entity);
        }
        public override async Task<IActionResult> AddRangeAsync(List<Whatsapp> entities)
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

        [HttpGet("GetWhatsAppByPhoneNumber")]        
        public async Task<IActionResult> GetWhatsAppByPhoneNumber([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _whatsappService.GetWhatsAppByPhoneNumber(model), IsSuccess = true });
        }        


        [HttpDelete("DeleteGroupData")]        
        public async Task<IActionResult> DeleteGroupData([FromBody] GetDeviceIdAndNameViewModel model)
        {
            var result = await _whatsappService.DeleteGroupData(model);
            
            if (result == false)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    Data = result,
                    IsSuccess = false,
                    Message = "No data found to delete."
                });
            }

            return new OkObjectResult(new ResponseMessageViewModel
            {
                Data = result,
                IsSuccess = true,
                Message = "Data deleted successfully."
            });
        }
    }
}
