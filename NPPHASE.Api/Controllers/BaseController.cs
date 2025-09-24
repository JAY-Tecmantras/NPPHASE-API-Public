using Ganss.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NPOI.XSSF.Streaming.Values;
using NPPHASE.Data.Enum;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseController<T> : ControllerBase where T : class, new()
    {
        private readonly IService<T> _service;
        private readonly IDeviceUserServices _deviceUserServices;

        public BaseController(IService<T> service, IDeviceUserServices deviceUserServices)
        {
            _service = service;
            _deviceUserServices = deviceUserServices;
        }
        [HttpGet]
        public virtual async Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(await _service.GetAll(model));
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById([FromRoute][Required] int id)
        {
            return new OkObjectResult(await _service.GetByIdAsync(id));
        }
        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody][Required] T entity)
        {
            return new OkObjectResult(await _service.Create(entity));
        }
        [HttpPut]
        public virtual IActionResult Put([FromBody][Required] T entity)
        {
            return new OkObjectResult(_service.Update(entity));
        }
        [HttpPost("UpdateRange")]
        public virtual IActionResult UpdateRange(List<T> entities)
        {
            return new OkObjectResult(_service.UpdateRange(entities));
        }
        [HttpPost("AddRangeAsync")]
        public virtual async Task<IActionResult> AddRangeAsync(List<T> entities)
        {
            return new OkObjectResult(await _service.AddRangeAsync(entities));
        }
        [HttpPost("DeleteRange")]
        public virtual async Task<IActionResult> DeleteRange(List<int> ids)
        {
            return new OkObjectResult(await _service.DeleteRange(ids));
        }

        [HttpPost("BulkImport")]
        public async Task<IActionResult> BulkImport([Required] IFormFile file, [FromQuery] int deviceUserId)
        {
            using (var stream = file.OpenReadStream())
            {
                var fileMapper = new ExcelMapper(stream);
                var rows = fileMapper.Fetch().ToList();
                var entityList = new List<T>();
                foreach (var row in rows)
                {
                    var rowData = new Dictionary<string, object>();
                    var rowDict = row as IDictionary<string, object>;

                    if (rowDict != null)
                    {
                        foreach (var kvp in rowDict)
                        {
                            if(kvp.Value != null)
                            {
                                if (double.TryParse(kvp.Value.ToString(), out _))
                                {
                                    rowData[kvp.Key] = kvp.Value;
                                }
                                else if (DateTime.TryParse(kvp.Value.ToString(), out DateTime result))
                                {
                                    rowData[kvp.Key] = result;
                                }
                                else
                                {
                                    rowData[kvp.Key] = kvp.Value;
                                }
                            }
                        }
                        if(deviceUserId > 0 ) 
                        {
                            rowData.Add("DeviceUserId", deviceUserId);
                        }
                        rowData.Remove(rowDict.FirstOrDefault().Key);
                    }

                    var entity = ConvertDictionaryToEntityList<T>(rowData);
                    entityList.Add(entity);
                }

                return new OkObjectResult(await _service.AddRangeAsync(entityList));
            }
        }

        private static T ConvertDictionaryToEntityList<T>(Dictionary<string, object> dictionary) where T : new()
        {
            T entity = new T();
            var entityType = typeof(T);
            foreach (var entry in dictionary)
            {
                var property = entityType.GetProperty(entry.Key, BindingFlags.Public | BindingFlags.Instance);
                if (property != null && property.CanWrite)
                {
                    object? value = entry.Value;
                   
                    switch (value)
                    {
                        case var _ when entry.Key.Contains("Type"):

                            if (property.PropertyType == typeof(IncomingOutgoingTypes?)|| property.PropertyType == typeof(IncomingOutgoingTypes))
                            {
                                if (Enum.TryParse(value.ToString(), out IncomingOutgoingTypes ioType)
                                     && Enum.IsDefined(typeof(IncomingOutgoingTypes), ioType))
                                {
                                    value = ioType;
                                }
                                else
                                {
                                    value = IncomingOutgoingTypes.Outgoing;
                                }
                            }
                            else if (property.PropertyType == typeof(SmsType))
                            {
                                if (Enum.TryParse(value.ToString(), out SmsType smsType)
                                    && Enum.IsDefined(typeof(SmsType), smsType))
                                {
                                    value = smsType; 
                                }
                                else
                                {
                                    value = SmsType.Receive;
                                }
                            }

                            break;

                        case var _ when value is DateTime || value is DateTimeOffset ?:
                            value = property.PropertyType == typeof(string)
                                ? value.ToString()
                                : value is DateTime dt ? new DateTimeOffset(dt) : (DateTimeOffset)value;
                            break;

                        case var _ when value is int || value is int?:
                            value = Convert.ToInt32(value.ToString());
                            break;                      
                                
                        default:
                             value = Convert.ChangeType(value, property.PropertyType);
                             break;
                    }                                                           
                    property.SetValue(entity, value);
                }
            }
            return entity;
        }

        [NonAction]
        public ResponseMessageViewModel GetDeviceId()
        {
            Request.Headers.TryGetValue("IMEINumber", out StringValues imeiNumber);
            int? DeviceId = _deviceUserServices.GetDeviceUserIdByImemiNumber(imeiNumber);
            if (string.IsNullOrEmpty(imeiNumber))
            {
                return new ResponseMessageViewModel { Message = "Please add header imeinumber first.", IsSuccess = false };
            }
            else if (!DeviceId.HasValue)
            {
                return new ResponseMessageViewModel { Message = "Please add device User first.", IsSuccess = false };

            }
            else
            {
                return new ResponseMessageViewModel { Data = DeviceId, IsSuccess = true };

            }
        }
    }
}
