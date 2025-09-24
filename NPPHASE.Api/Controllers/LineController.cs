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

    public class LineController : BaseController<Line>
    {
        private readonly ILineService _lineService;
        public LineController(IService<Line> service, ILineService lineService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _lineService = lineService;
        }
        public override async Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _lineService.GetAll(model), IsSuccess = true });
        }
        [HttpGet("GetLineByContactPersonName")]        
        public async Task<IActionResult> GetLineByContactPersonName([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _lineService.GetLineByContactPersonName(model), IsSuccess = true });
        }
        public override async Task<IActionResult> Post([FromBody, Required] Line entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return await base.Post(entity);
        }
        public override async Task<IActionResult> AddRangeAsync(List<Line> entities)
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
            var result = await _lineService.DeleteGroupData(model);

            if (result == false)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    Data = result,
                    IsSuccess = false,
                    Message = "No data found Please enter your number"
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
