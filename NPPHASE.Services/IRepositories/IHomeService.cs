using NPPHASE.Data.ViewModel;

namespace NPPHASE.Services.IRepositories
{
    public interface IHomeService
    {
        Task<PagedListViewModel<DashBoardDetailsViewModel>> GetAllDeviceUser();
    }
}
