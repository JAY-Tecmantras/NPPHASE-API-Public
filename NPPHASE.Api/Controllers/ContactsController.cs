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

    public class ContactsController : BaseController<Contact>
    {
        private readonly IContactsService _contactsService;

        public ContactsController(IService<Contact> service, IContactsService contactsService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _contactsService = contactsService;
        }
        public async override Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _contactsService.GetAll(model), IsSuccess = true });

        }
        public override async Task<IActionResult> Post([FromBody, Required] Contact entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.MobileNumber = !string.IsNullOrEmpty(entity.MobileNumber) ? entity.MobileNumber.Trim().Replace(" ", "") : entity.MobileNumber;
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return await base.Post(entity);
        }
        public override IActionResult Put([FromBody, Required] Contact entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.MobileNumber = !string.IsNullOrEmpty(entity.MobileNumber) ? entity.MobileNumber.Trim().Replace(" ", "") : entity.MobileNumber;
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return base.Put(entity);
        }
        public override async Task<IActionResult> AddRangeAsync(List<Contact> entities)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entities.ForEach(x =>
            {
                x.MobileNumber = !string.IsNullOrEmpty(x.MobileNumber) ? x.MobileNumber.Trim().Replace(" ", "") : null;
                x.DeviceUserId = Convert.ToInt32(deviceData.Data);
            });
            return await base.AddRangeAsync(entities);
        }
        public override IActionResult UpdateRange(List<Contact> entities)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entities.ForEach(x =>
            {
                x.MobileNumber = !string.IsNullOrEmpty(x.MobileNumber) ? x.MobileNumber.Trim().Replace(" ", "") : null;
                x.DeviceUserId = Convert.ToInt32(deviceData.Data);
            });
            return base.UpdateRange(entities);
        }
        [HttpPost("RemoveContacts")]
        public async Task<IActionResult> RemoveContacts()
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            return new OkObjectResult(new ResponseMessageViewModel { IsSuccess = await _contactsService.RemoveContacts(Convert.ToInt32(deviceData.Data)) });
        }

        [HttpGet("GetContactDetailsByExcel")]        
        public async Task<IActionResult> GetContactDetailsByExcel([FromQuery] GetAllRequestViewModel model)
        {
            byte[] excelData = await _contactsService.GetContactDetailsByExcel(model);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Contact_{model.DeviceUserId}_{DateTime.UtcNow}.xlsx");

        }
    }
}
