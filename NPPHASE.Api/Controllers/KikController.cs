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

    public class KikController : BaseController<Kik>
    {
        private readonly IKikService _kikService;
        public KikController(IService<Kik> service, IKikService kikService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _kikService = kikService;
        }
        public override async Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _kikService.GetAll(model), IsSuccess = true });
        }
        [HttpGet("GetKikByContactPersonName")]        
        public async Task<IActionResult> GetKikByContactPersonName([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _kikService.GetKikByContactPersonName(model), IsSuccess = true });
        }
        public override async Task<IActionResult> Post([FromBody, Required] Kik entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return await base.Post(entity);
        }
        public override async Task<IActionResult> AddRangeAsync(List<Kik> entities)
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

        [HttpDelete("DeleteGroupData")]       
        public async Task<IActionResult> DeleteGroupData([FromBody] GetDeviceIdAndNameViewModel model)
        {
            var result = await _kikService.DeleteGroupData(model);

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
