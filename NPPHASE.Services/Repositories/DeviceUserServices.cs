using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class DeviceUserServices : IDeviceUserServices
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<DeviceUser> _deviceUserRepository;
        private readonly IService<DeviceUser> _service;
        private readonly IRepository<DeviceData> _deviceDataRepository;


        public DeviceUserServices(IUnitofWork unitofWork, IService<DeviceUser> service)
        {
            _unitofWork = unitofWork;
            _deviceDataRepository = _unitofWork.GetRepository<DeviceData>();
            _deviceUserRepository = _unitofWork.GetRepository<DeviceUser>();
            _service = service;
        }

        public async Task<PagedListViewModel<DeviceUser>> GetAll(GetAllRequestViewModel model)
        {
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }

            return new PagedListViewModel<DeviceUser>
            {
                TotalCount = result.Count(),
                ListResponse = await pagedResult.ToListAsync()
            };
        }

        public async Task<DeviceUserViewModel> GetDeviceById(int id)
        {
            var deviceDetails = await _deviceUserRepository.GetAll().Where(x => !x.IsDeleted).GroupJoin(_deviceDataRepository.GetAll(),
            dUser => dUser.DeviceUserId, dData => dData.DeviceUserId,
            (dUser, dData) => new
            {
                deviceUser = dUser,
                deviceData = dData.DefaultIfEmpty(),
            }).Where(x => x.deviceUser.DeviceUserId == id).SelectMany(
                result => result.deviceData,
                (result, deviceData) => new DeviceUserViewModel
                {
                    DeviceUserId = result.deviceUser.DeviceUserId,
                    DeviceName = result.deviceUser.DeviceName,
                    Model = result.deviceUser.Model,
                    OS = result.deviceUser.OS,
                    Version = result.deviceUser.Version,
                    IMEINumber = result.deviceUser.IMEINumber,
                    BatteryPerc = deviceData.BatteryPercentage.ToString(),
                    IsConnectedWithWifi = deviceData.IsConnectedWithWifi,
                    DeviceStatus = deviceData.Status.Value.ToString(),
                    AlacExtractionProgress = result.deviceUser.AlacExtractionProgress,
                    AlacAllotmentProgress = result.deviceUser.AlacAllotmentProgress,
                    PrivateKeyExtractionProgress = result.deviceUser.PrivateKeyExtractionProgress,
                    AlacDecryptionProgress = result.deviceUser.AlacDecryptionProgress,
                    PhaseMonitor = result.deviceUser.PhaseMonitor
                }).FirstOrDefaultAsync();

            return deviceDetails;
        }

        public async Task<DeviceUser> GetDeviceUserDetails(string userId, string iMEINumber)
        {
            return await _deviceUserRepository.GetAll().Where(x => x.UserId == userId && x.IMEINumber == iMEINumber && !x.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<ResponseMessageViewModel> UpdateDeviceToken(UpdateDeviceTokenViewModel viewModel)
        {
            var deviceUser = await _deviceUserRepository.GetByIdAsync(viewModel.DeviceUserId);
            if (deviceUser != null)
            {
                deviceUser.DeviceToken = viewModel.DeviceToken;
                _deviceUserRepository.Update(deviceUser);
                _ = _unitofWork.commit();
                return new ResponseMessageViewModel
                {
                    IsSuccess = true
                };
            }
            return new ResponseMessageViewModel
            {
                IsSuccess = false,
                Message = "Device not found"
            };
        }

        public async Task<ResponseMessageViewModel> GetDeviceToken(int deviceUserId)
        {
            try
            {
                ResponseMessageViewModel response = new ResponseMessageViewModel();
                var deviceUser = await _deviceUserRepository.GetAll().Where(x => x.DeviceUserId == deviceUserId && !x.IsDeleted).FirstOrDefaultAsync();
                if (deviceUser != null && !string.IsNullOrEmpty(deviceUser.DeviceToken))
                {
                    response.Data = deviceUser.DeviceToken;
                    response.IsSuccess = true;
                    return response;

                }
                response.IsSuccess = false;
                response.Message = "User not found";
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int? GetDeviceUserIdByImemiNumber(string iMEINUmber)
        {
            return _deviceUserRepository.GetAll().Where(x => x.IMEINumber == iMEINUmber).OrderByDescending(t => t.CreationDate).Select(y => y.DeviceUserId).FirstOrDefault();
        }

        public async Task<bool> ResetAlacProgressColumn(int deviceUserId)
        {
            var user = await _deviceUserRepository.GetByIdAsync(deviceUserId);
            if (user == null) return false;

            user.AlacExtractionProgress = 0;
            user.AlacAllotmentProgress = 0;
            user.PrivateKeyExtractionProgress = 0;
            user.AlacDecryptionProgress = 0;
            user.PhaseMonitor = 0;

            _ = _unitofWork.commit();
            return true;
        }
    }
}
