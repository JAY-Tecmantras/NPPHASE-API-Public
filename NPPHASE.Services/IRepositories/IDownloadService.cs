using NPPHASE.Data.Enum;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;

namespace NPPHASE.Services.IRepositories
{
    public interface IDownloadService
    {
        Task<int> CreateUpdate(bool isAndroid);
        Task<List<DownloadViewModel>> GetAllDownloadYearAndMonthWise(int year);

    }
}
