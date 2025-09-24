using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Common;
using NPPHASE.Data.Context;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDeviceDataService _deviceDataService;
        private readonly IExceptionLoggerServices _exceptionLoggerServices;
        private readonly UserManager<User> _userManager;
        private readonly IService<DeviceUser> _deviceUserService;
        private readonly IRepository<DeviceData> _repository;
        private readonly IRepository<DeviceUser> _deviceUserrepository;
        private readonly IUnitofWork _unitofWork;
        private readonly IDeviceUserCleanupService _deviceUserCleanupService;

        public UsersController(
            IDeviceDataService deviceDataService,
            IExceptionLoggerServices exceptionLoggerServices,
            UserManager<User> userManager,
            IUnitofWork unitofWork,            
            IService<DeviceUser> deviceUserService,
            IDeviceUserCleanupService deviceUserCleanupService)
        {
            _deviceDataService = deviceDataService;
            _exceptionLoggerServices = exceptionLoggerServices;
            _userManager = userManager;
            _unitofWork = unitofWork;
            _repository = _unitofWork.GetRepository<DeviceData>();
            _deviceUserrepository = _unitofWork.GetRepository<DeviceUser>();
            _deviceUserService = deviceUserService;
            _deviceUserCleanupService = deviceUserCleanupService;
        }

        // User Email Validation Helper Method
        private bool IsValidEmail(string email, ref string validationMessage)
        {
            validationMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(email))
            {
                validationMessage = "Email cannot be empty or null.";
                return false;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    validationMessage = "Invalid email format.";
                    return false;
                }
                return true;
            }
            catch
            {
                validationMessage = "Invalid email format.";
                return false;
            }
        }

        // User Create Helper Method
        private Task<User> CreateUserAsync(string email, string name, ref string message)
        {
            message = string.Empty;

            var existingUserTask = _userManager.FindByEmailAsync(email);
            if (existingUserTask.Result != null)
            {
                message = "A user with this email id is already exists.";
                return Task.FromResult<User>(null);
            }

            var newUser = new User
            {
                UserName = email,
                Email = email,
                PasswordHash = email,
                Name = name,
                EmailConfirmed = true,
                CreationDate = DateTimeOffset.UtcNow,
            };

            var createUserResultTask = _userManager.CreateAsync(newUser, newUser.PasswordHash);
            if (!createUserResultTask.Result.Succeeded)
            {
                message = $"Failed to create user: {string.Join(", ", createUserResultTask.Result.Errors.Select(e => e.Description))}";
                return Task.FromResult<User>(null);
            }

            var roleAssignResultTask = _userManager.AddToRoleAsync(newUser, "User");
            if (!roleAssignResultTask.Result.Succeeded)
            {
                message = $"Failed to assign role: {string.Join(", ", roleAssignResultTask.Result.Errors.Select(e => e.Description))}";
                return Task.FromResult<User>(null);
            }

            message = "User created successfully.";
            return Task.FromResult(newUser);
        }

        // DeviceUser Create Helper Method
        private Task<DeviceUser> CreateDeviceUserAsync(DeviceUserDataViewModel model, string userId, ref string message)
        {
            message = string.Empty;

            var deviceUser = new DeviceUser
            {
                DeviceUniqueId = CommonFunctions.GenerateAlphabaticNumber(6),
                DeviceName = model.DeviceName,
                Model = model.Model,
                OS = model.OS,
                Version = model.Version,
                IMEINumber = model.IMEINumber,
                UserId = userId,
                CreationDate = DateTimeOffset.UtcNow,
            };

            var createDeviceUserTask = _deviceUserService.Create(deviceUser);
            if (createDeviceUserTask.Result == null)
            {
                message = "Failed to create device user.";
                return Task.FromResult<DeviceUser>(null);
            }

            message = "Device user created successfully.";
            return createDeviceUserTask;
        }

        // DeviceData Create Helper Method
        private Task<DeviceData> CreateDeviceDataAsync(DeviceUserDataViewModel model, int deviceUserId, ref string message)
        {
            message = string.Empty;

            var deviceData = new DeviceData
            {
                DeviceUserId = deviceUserId,
                IsConnectedWithWifi = model.IsConnectedWithWifi,
                CreationDate = DateTimeOffset.UtcNow,
                BatteryPercentage = model.BatteryPercentage,
                Status = Data.Enum.DeviceStatus.Active
            };

            var createDeviceDataTask = _deviceDataService.CreateUpdate(deviceData);
            if (createDeviceDataTask.Result == 0)
            {
                message = "Failed to create device data.";
                return Task.FromResult<DeviceData>(null);
            }

            message = "Device Data created successfully";
            return Task.FromResult(deviceData);
        }

        // CRUD API

        [HttpPost("AddAll")]
        [Authorize]
        public async Task<IActionResult> CreateAllEntities([FromBody] DeviceUserDataViewModel model)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseMessageViewModel { IsSuccess = false, Message = "Invalid model data" });
            }

            //Validate email
            string validationMessage = string.Empty;

            if (!IsValidEmail(model.Email, ref validationMessage))
            {
                return BadRequest(new ResponseMessageViewModel
                {
                    Data = false,
                    IsSuccess = false,
                    Message = validationMessage
                });
            }
            
            try
            {
                // New User Create
                var user = await CreateUserAsync(model.Email, model.Name, ref validationMessage);
                if (user == null)
                {
                    return BadRequest(new ResponseMessageViewModel { Data = false, IsSuccess = false, Message = validationMessage });
                }

                // DeviceUser Create
                var deviceUser = await CreateDeviceUserAsync(model, user.Id, ref validationMessage);
                if (deviceUser == null)
                {
                    return BadRequest(new ResponseMessageViewModel { Data = false, IsSuccess = false, Message = validationMessage });
                }

                // DeviceData Create                              
                var deviceData = await CreateDeviceDataAsync(model, deviceUser.DeviceUserId, ref validationMessage);
                if (deviceData == null)
                {
                    return BadRequest(new ResponseMessageViewModel { Data = false, IsSuccess = false, Message = validationMessage });
                }

                return Ok(new ResponseMessageViewModel
                {
                    Data = true,
                    IsSuccess = true,
                    Message = "User, Device User, Device Data Added Successfully"
                });
            }
            catch (Exception ex)
            {
                _exceptionLoggerServices.InsertErrorLog(new Exceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Timestamp = DateTimeOffset.UtcNow,
                    ScreenName = "CRUD Device",
                });

                return StatusCode(500, new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }


        [HttpPut("UpdateAll/{deviceUserId}")]
        [Authorize]
        public async Task<IActionResult> UpdateAllEntities(int deviceUserId, [FromBody] DeviceUserDataViewModel model)
        {
            string validationMessage = string.Empty;

            if (model == null)
            {
                return BadRequest(new ResponseMessageViewModel { IsSuccess = false, Message = "Invalid input data" });
            }

            // Validate email
            if (!IsValidEmail(model.Email, ref validationMessage))
            {
                return BadRequest(new ResponseMessageViewModel
                {
                    Data= false,
                    IsSuccess = false,
                    Message = validationMessage
                });
            }            

            try
            {
                // Fetch the DeviceUser
                var deviceUser = await _deviceUserrepository.GetByIdAsync(deviceUserId);
                if (deviceUser == null)
                {
                    return NotFound(new ResponseMessageViewModel { IsSuccess = false, Message = "Device user id not found" });
                }

                // Fetch the associated User
                var user = await _userManager.FindByIdAsync(deviceUser.UserId);
                if (user == null)
                {
                    return NotFound(new ResponseMessageViewModel { IsSuccess = false, Message = "User not found" });
                }                

                // Check for duplicate email
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    return BadRequest(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "A user with this email id already exists."
                    });
                }

                // Fetch DeviceData
                var deviceData = await _repository.GetAll()
                    .FirstOrDefaultAsync(x => x.DeviceUserId == deviceUser.DeviceUserId && !x.IsDeleted);

                // Track update needs
                bool userNeedsUpdate = !(user.Email == model.Email && user.Name == model.Name);
                bool deviceUserNeedsUpdate = !(deviceUser.DeviceName == model.DeviceName &&
                                               deviceUser.Model == model.Model &&
                                               deviceUser.OS == model.OS &&
                                               deviceUser.Version == model.Version);
                bool deviceDataNeedsUpdate = deviceData != null &&
                                              !(deviceData.IsConnectedWithWifi == model.IsConnectedWithWifi &&
                                                deviceData.BatteryPercentage == model.BatteryPercentage);

                // Perform updates if necessary
                if (userNeedsUpdate)
                {
                    // Update User details
                    user.Email = model.Email;                    
                    user.UserName = model.Email;
                    user.Name = model.Name;                   
                    user.ModificationDate = DateTimeOffset.UtcNow;

                    var userUpdateResult = await _userManager.UpdateAsync(user);
                    if (!userUpdateResult.Succeeded)
                    {
                        return BadRequest(new ResponseMessageViewModel
                        {
                            IsSuccess = false,
                            Message = $"Error updating user: {string.Join(", ", userUpdateResult.Errors.Select(e => e.Description))}"
                        });
                    }
                }

                if (deviceUserNeedsUpdate)
                {
                    // Update DeviceUser details
                    deviceUser.DeviceName = model.DeviceName;
                    deviceUser.Model = model.Model;
                    deviceUser.OS = model.OS;
                    deviceUser.Version = model.Version;
                    deviceUser.ModificationDate = DateTimeOffset.UtcNow;

                    var deviceUserUpdateResult = _deviceUserService.Update(deviceUser);
                    if (deviceUserUpdateResult == null)
                    {
                        return BadRequest(new ResponseMessageViewModel { IsSuccess = false, Message = "Error updating device user" });
                    }
                }

                if (deviceDataNeedsUpdate && deviceData != null)
                {
                    // Update DeviceData
                    deviceData.IsConnectedWithWifi = model.IsConnectedWithWifi;
                    deviceData.BatteryPercentage = model.BatteryPercentage;
                    deviceData.ModificationDate = DateTimeOffset.UtcNow;

                    var deviceDataUpdateResult = await _deviceDataService.CreateUpdate(deviceData);
                    if (deviceDataUpdateResult == 0)
                    {
                        return BadRequest(new ResponseMessageViewModel { IsSuccess = false, Message = "Error updating device data" });
                    }
                }

                // If no updates were required
                if (!userNeedsUpdate && !deviceUserNeedsUpdate && !deviceDataNeedsUpdate)
                {
                    return Ok(new ResponseMessageViewModel
                    {
                        IsSuccess = true,
                        Message = "No changes were made as all data is already up-to-date."
                    });
                }

                return Ok(new ResponseMessageViewModel
                {
                    Data = true,
                    IsSuccess = true,
                    Message = "User, Device user, and Device data updated successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseMessageViewModel { IsSuccess = false, Message = $"Error occurred: {ex.Message}" });
            }
        }
        

        [HttpDelete("DeleteAll/{deviceUserId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAllEntities(int deviceUserId)
        {
            try
            {
                // Fetch the DeviceUser
                var deviceUser = await _deviceUserrepository.GetByIdAsync(deviceUserId);
                if (deviceUser == null)
                    return NotFound(new ResponseMessageViewModel { IsSuccess = false, Message = "Device user  not found" });

                // Fetch the associated User
                var user = await _userManager.FindByIdAsync(deviceUser.UserId);
                if (user == null)
                    return NotFound(new ResponseMessageViewModel { IsSuccess = false, Message = "User not found" });

                //Delete all related entities first
                await _deviceUserCleanupService.CleanupRelatedEntitiesAsync(deviceUserId);

                // Delete DeviceData
                var deviceData = await _repository.GetAll().FirstOrDefaultAsync(x => x.DeviceUserId == deviceUser.DeviceUserId && !x.IsDeleted);

                if (deviceData != null)
                {
                    _repository.Delete(deviceData);
                }

                // Delete DeviceUser
                var deviceUserDeleteResult = _deviceUserService.HardDelete(deviceUser.DeviceUserId);
                //if (!deviceUserDeleteResult)
                //    return BadRequest(new ResponseMessageViewModel { IsSuccess = false, Message = "Error deleting device user" });

                // Delete User data
                var userDeleteResult = await _userManager.DeleteAsync(user);
                if (!userDeleteResult.Succeeded)
                {
                    return BadRequest(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = $"Error deleting user: {string.Join(", ", userDeleteResult.Errors.Select(e => e.Description))}"
                    });
                }

                return Ok(new ResponseMessageViewModel
                {
                    Data = true,
                    IsSuccess = true,
                    Message = "User, device user, and device data deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseMessageViewModel { IsSuccess = false, Message = $"Error occurred: {ex.Message}" });
            }
        }

    }
}

