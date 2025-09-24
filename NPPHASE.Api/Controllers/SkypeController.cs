using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using NPPHASE.Services.Repositories;
using System.ComponentModel.DataAnnotations;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SkypeController : BaseController<Skype>
    {
        private readonly ISkypeService _skypeService;

        public SkypeController(IService<Skype> service, ISkypeService skypeService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _skypeService = skypeService;
        }
        public override async Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _skypeService.GetAll(model), IsSuccess = true });
        }
        public override async Task<IActionResult> Post([FromBody, Required] Skype entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return await base.Post(entity);
        }
        public override async Task<IActionResult> AddRangeAsync(List<Skype> entities)
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

        [HttpGet("GetSkypeByContactPersonName")]        
        public async Task<IActionResult> GetSkypeByContactPersonName([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _skypeService.GetSkypeByContactPersonName(model), IsSuccess = true });
        }

        [HttpDelete("DeleteGroupData")]        
        public async Task<IActionResult> DeleteGroupData([FromBody] GetDeviceIdAndNameViewModel model)
        {
            var result = await _skypeService.DeleteGroupData(model);

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
