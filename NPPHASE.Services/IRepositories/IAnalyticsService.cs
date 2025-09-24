using NPPHASE.Data.ViewModel;

namespace NPPHASE.Services.IRepositories
{
    public interface IAnalyticsService
    {
        Task<AnalyticsViewModel> GetAnalyTicsDetails();
        Task<AnalyticsViewModel> GetAnalyTicsDetailsByYear(int year);
    }
}
