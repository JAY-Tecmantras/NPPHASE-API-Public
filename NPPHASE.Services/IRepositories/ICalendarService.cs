using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;

namespace NPPHASE.Services.IRepositories
{
    public interface ICalendarService
    {
        Task<PagedListViewModel<Calendar>> GetAll(GetAllRequestViewModel model);
    }
}
