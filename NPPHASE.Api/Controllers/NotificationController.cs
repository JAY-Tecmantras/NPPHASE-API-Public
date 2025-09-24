using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IDeviceUserServices _deviceUserServices;

        public NotificationController(IDeviceUserServices deviceUserServices)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile("tec-mantras-projcet-001-firebase-adminsdk-ala8o-e12ba5b7c9.json")
                });
            }
            _deviceUserServices = deviceUserServices;
        }
        [HttpPost("SendNotification")]
        [Authorize]

        public async Task<IActionResult> SendNotification(SendNotificationViewModel request)
        {
            try
            {
                var deviceUser = await _deviceUserServices.GetDeviceToken(request.DeviceUserId);
                if (deviceUser.IsSuccess)
                {


                    var message = new Message
                    {
                        Notification = new Notification
                        {
                            Title = request.Title,
                            Body = request.Message,
                        },
                        Data = new Dictionary<string, string>
                        {
                            { request.Key, request.Value },
                        },
                        Token = deviceUser.Data.ToString(),
                    };
                    var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                    return new OkObjectResult(response);
                }
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Device not found"
                });


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
