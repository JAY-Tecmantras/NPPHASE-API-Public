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

    public class FacebookController : BaseController<Facebook>
    {
        private readonly IFaceBookService _faceBookService;

        public FacebookController(IService<Facebook> service, IFaceBookService faceBookService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _faceBookService = faceBookService;
        }
        public override async Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _faceBookService.GetAll(model), IsSuccess = true });
        }
        public override async Task<IActionResult> Post([FromBody, Required] Facebook entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return await base.Post(entity);
        }
        public override async Task<IActionResult> AddRangeAsync(List<Facebook> entities)
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

        [HttpGet("GetFaceBookByContactPersonName")]        
        public async Task<IActionResult> GetFaceBookByContactPersonName([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _faceBookService.GetFaceBookByContactPersonName(model), IsSuccess = true });
        }

        [HttpDelete("DeleteGroupData")]        
        public async Task<IActionResult> DeleteGroupData([FromBody] GetDeviceIdAndNameViewModel model)
        {
            var result = await _faceBookService.DeleteGroupData(model);

            if (result == false)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    Data = result,
                    IsSuccess = false,
                    Message = "No data found to delete. Please enter number."
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
