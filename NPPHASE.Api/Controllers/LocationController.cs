using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using System.ComponentModel.DataAnnotations;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class LocationController : BaseController<Location>
    {
        private readonly ILocationService _locationService;

        public LocationController(IService<Location> service, ILocationService locationService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _locationService = locationService;
        }
        public async override Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _locationService.GetAll(model), IsSuccess = true });

        }
        public override async Task<IActionResult> Post([FromBody, Required] Location entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return await base.Post(entity);
        }
        public override IActionResult Put([FromBody, Required] Location entity)
        {
            var a = "test;";
            var abc = a;
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return base.Put(entity);
        }
        public override async Task<IActionResult> AddRangeAsync(List<Location> entities)
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
        public override IActionResult UpdateRange(List<Location> entities)
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
            return base.UpdateRange(entities);
        }
    }
}
