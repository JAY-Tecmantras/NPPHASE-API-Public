using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using System.ComponentModel.DataAnnotations;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CardDetailController : BaseController<CardDetail>
    {
        private readonly ICardDetailService _cardDetailService;
        private readonly IExceptionLoggerServices _exceptionLoggerServices;

        public CardDetailController(IService<CardDetail> service, ICardDetailService cardDetailService, IExceptionLoggerServices exceptionLoggerService,
            IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _cardDetailService = cardDetailService;
            _exceptionLoggerServices = exceptionLoggerService;
        }
        public async override Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _cardDetailService.GetAll(model), IsSuccess = true });
        }
        [HttpPost("CreateUpdateCardDetails")]
        public async Task<IActionResult> CreateUpdateCardDetails(CardDetail cardDetail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ResponseMessageViewModel deviceData = GetDeviceId();
                    if (deviceData.IsSuccess == false)
                    {
                        return new OkObjectResult(deviceData);
                    }
                    cardDetail.DeviceUserId = Convert.ToInt32(deviceData.Data);

                    int result = await _cardDetailService.CreateUpdateCardDetails(cardDetail);
                    if (result == 1)
                    {
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            IsSuccess = true
                        });
                    }
                    else
                    {
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            IsSuccess = false,
                            Message = "Error occured while creating or updating card details"
                        });
                    }
                }
                else
                {
                    return new OkObjectResult(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "Model is not validate"
                    });
                }
            }
            catch (Exception ex)
            {
                var exception = new Exceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Timestamp = DateTimeOffset.UtcNow,
                    ScreenName = "CardDetail/CreateUpdateCardDetails",
                };
                _exceptionLoggerServices.InsertErrorLog(exception);
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Error occured while creating or updating card details"
                });
            }
        }
        public override async Task<IActionResult> Post([FromBody, Required] CardDetail entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return await base.Post(entity);
        }
        public override IActionResult Put([FromBody, Required] CardDetail entity)
        {
            ResponseMessageViewModel deviceData = GetDeviceId();
            if (deviceData.IsSuccess == false)
            {
                return new OkObjectResult(deviceData);
            }
            entity.DeviceUserId = Convert.ToInt32(deviceData.Data);
            return base.Put(entity);
        }
        public override async Task<IActionResult> AddRangeAsync(List<CardDetail> entities)
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
        public override IActionResult UpdateRange(List<CardDetail> entities)
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
