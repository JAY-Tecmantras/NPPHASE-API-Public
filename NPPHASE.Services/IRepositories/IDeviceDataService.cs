using NPPHASE.Data.Enum;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;

namespace NPPHASE.Services.IRepositories
{
    public interface IDeviceDataService
    {
        Task<int> CreateUpdate(DeviceData model);
        Task<PagedListViewModel<DeviceData>> GetAll(GetAllRequestViewModel model);
        Task<int> UpdateStatus(int DeviceUserId, DeviceStatus status);
    }
}
