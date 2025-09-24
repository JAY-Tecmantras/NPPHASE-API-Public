using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Enum;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class DeviceDataService : IDeviceDataService
    {
        private readonly IRepository<DeviceData> repository;
        private readonly IUnitofWork _unitofWork;
        private readonly IService<DeviceData> _service;

        public DeviceDataService(IUnitofWork unitofWork, IService<DeviceData> service)
        {
            _unitofWork = unitofWork;
            repository = _unitofWork.GetRepository<DeviceData>();
            _service = service;
        }
        public async Task<int> CreateUpdate(DeviceData model)
        {
            var userDeviceData = await repository.GetAll().Where(x => !x.IsDeleted && x.DeviceUserId == model.DeviceUserId).FirstOrDefaultAsync();
            if (userDeviceData != null)
            {
                userDeviceData.BatteryPercentage = model.BatteryPercentage;
                userDeviceData.IsConnectedWithWifi = model.IsConnectedWithWifi;
                repository.Update(userDeviceData);
            }
            else
            {
                await repository.Add(model);
            }
            return _unitofWork.commit();
        }

        public async Task<PagedListViewModel<DeviceData>> GetAll(GetAllRequestViewModel model)
        {
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }

            return new PagedListViewModel<DeviceData>
            {
                TotalCount = result.Count(),
                ListResponse = await pagedResult.ToListAsync()
            };
        }
        public async Task<int> UpdateStatus(int deviceUserId, DeviceStatus status)
        {
            var device = await repository.GetAll()
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.DeviceUserId == deviceUserId);

            if (device == null)
            {
                return -1;
            }
                
            device.Status = status;
            repository.Update(device);

            return _unitofWork.commit();
        }
    }
}
