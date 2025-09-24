using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;

namespace NPPHASE.Services.IRepositories
{
    public interface ISurroundRecordingService
    {
        Task<PagedListViewModel<SurroundRecordingViewModel>> GetAll(GetAllRequestViewModel model);
        Task<SurroundRecordings> AddSurroundRecording(SurroundRecordingViewModel surroundRecordingViewModel);
    }
}
