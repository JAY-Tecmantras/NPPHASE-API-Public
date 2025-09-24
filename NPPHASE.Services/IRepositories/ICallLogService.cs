using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;

namespace NPPHASE.Services.IRepositories
{
    public interface ICallLogService
    {
        Task<PagedListViewModel<CallLogViewModel>> GetAll(GetAllRequestViewModel model);
        Task<CallLog> AddCallLog(AddCallLogViewModel callLogViewModel);
        Task<byte[]> GetCallLogDetailsByExcel(GetAllRequestViewModel model);
    }
}
