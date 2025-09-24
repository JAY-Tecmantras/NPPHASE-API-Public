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

    public class SMSController : BaseController<SMSLog>
    {
        private readonly ISMSLogService _sMSLogService;
        public SMSController(IService<SMSLog> service, ISMSLogService sMSLogService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _sMSLogService = sMSLogService;
        }
        public override async Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _sMSLogService.GetAll(model), IsSuccess = true });

        }
        public override async Task<IActionResult> Post([FromBody, Required] SMSLog entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.Number = !string.IsNullOrEmpty(entity.Number) ? entity.Number.Trim() : null;
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return await base.Post(entity);
        }
        public override async Task<IActionResult> AddRangeAsync(List<SMSLog> entities)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entities.ForEach(x =>
            {
                x.Number = !string.IsNullOrEmpty(x.Number) ? x.Number.Trim() : null;
                x.DeviceUserId = Convert.ToInt32(deviceData.Data);
            });
            return await base.AddRangeAsync(entities);
        }
        [HttpGet("GetSmsLogDetailsByExcel")]        
        public async Task<IActionResult> GetSmsLogDetailsByExcel([FromQuery] GetAllRequestViewModel model)
        {
            byte[] excelData = await _sMSLogService.GetSmsLogDetailsByExcel(model);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"SmsLog_{model.DeviceUserId}_{DateTime.UtcNow}.xlsx");

        }

        [HttpDelete("DeleteGroupData")]       
        public async Task<IActionResult> DeleteGroupData([FromBody] GetDeviceIdAndNameViewModel model)
        {
            var result = await _sMSLogService.DeleteGroupData(model);

            if (result == false)
            {
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    Data = result,
                    IsSuccess = false,
                    Message = "Please enter proper phone number"
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
