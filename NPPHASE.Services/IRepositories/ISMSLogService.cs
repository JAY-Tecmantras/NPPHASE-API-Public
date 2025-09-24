using NPPHASE.Data.ViewModel;

namespace NPPHASE.Services.IRepositories
{
    public interface ISMSLogService
    {
        Task<PagedListViewModel<SmsViewModel>> GetAll(GetAllRequestViewModel model);
        Task<byte[]> GetSmsLogDetailsByExcel(GetAllRequestViewModel model);
        Task<bool> DeleteGroupData(GetDeviceIdAndNameViewModel model);
    }
}
