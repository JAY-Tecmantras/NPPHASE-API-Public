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
    public class CallLogController : BaseController<CallLog>
    {
        private readonly ICallLogService _callLogService;

        public CallLogController(IService<CallLog> service, ICallLogService callLogService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _callLogService = callLogService;
        }
       
        public async override Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _callLogService.GetAll(model), IsSuccess = true });
        }
        public override async Task<IActionResult> Post([FromBody, Required] CallLog entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }

            entity.Number = !string.IsNullOrEmpty(entity.Number) ? entity.Number.Trim().Replace(" ", "") : null;
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return await base.Post(entity);
        }

        public override IActionResult Put([FromBody, Required] CallLog entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.Number = !string.IsNullOrEmpty(entity.Number) ? entity.Number.Trim().Replace(" ", "") : null;
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return base.Put(entity);
        }
        public override async Task<IActionResult> AddRangeAsync(List<CallLog> entities)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entities.ForEach(x =>
            {
                x.Number = !string.IsNullOrEmpty(x.Number) ? x.Number.Trim().Replace(" ", "") : null;
                x.DeviceUserId = Convert.ToInt32(deviceData.Data);
            });
            return await base.AddRangeAsync(entities);
        }
        public override IActionResult UpdateRange(List<CallLog> entities)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entities.ForEach(x =>
            {
                x.Number = !string.IsNullOrEmpty(x.Number) ? x.Number.Trim().Replace(" ", "") : null;
                x.DeviceUserId = Convert.ToInt32(deviceData.Data);
            });
            return base.UpdateRange(entities);
        }
        [HttpPost("AddCallLog")]
        public async Task<IActionResult> AddCallLog([FromForm] AddCallLogViewModel entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.Number = !string.IsNullOrEmpty(entity.Number) ? entity.Number.Trim().Replace(" ", "") : null;
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _callLogService.AddCallLog(entity), IsSuccess = true });
        }
        [HttpGet("GetCallLogDetailsByExcel")]
        public async Task<IActionResult> GetCallLogDetailsByExcel([FromQuery] GetAllRequestViewModel model)
        {
            byte[] excelData = await _callLogService.GetCallLogDetailsByExcel(model);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"CallLog_{model.DeviceUserId}_{DateTime.UtcNow}.xlsx");

        }

    }
}
