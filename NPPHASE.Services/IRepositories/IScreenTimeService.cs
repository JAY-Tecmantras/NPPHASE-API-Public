using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;

namespace NPPHASE.Services.IRepositories
{
    public interface IScreenTimeService
    {
        Task<PagedListViewModel<ScreenTimeViewModel>> GetAll(GetAllRequestViewModel model);
        Task<int> CreateUpdateScreenTime(ScreenTimeViewModel model);
        Task<ScreenTimeAppNameTotalTimeViewModel> Top5AppAndScreenTime(GetAllRequestViewModel model);
    }
}
