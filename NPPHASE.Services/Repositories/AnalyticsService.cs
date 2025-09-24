using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.Enum;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using System.Globalization;

namespace NPPHASE.Services.Repositories
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<Download> _downloadRepository;
        private readonly IRepository<DeviceData> _deviceDataRepository;
        private readonly UserManager<User> _userManager;

        public AnalyticsService(IUnitofWork unitofWork, UserManager<User> userManager)
        {
            _unitofWork = unitofWork;
            _downloadRepository = _unitofWork.GetRepository<Download>();
            _deviceDataRepository = _unitofWork.GetRepository<DeviceData>();
            _userManager = userManager;
        }
        public async Task<AnalyticsViewModel> GetAnalyTicsDetails()
        {
            var deviceData = _deviceDataRepository.GetAll().Where(x => !x.IsDeleted);
            var download = _downloadRepository.GetAll().Where(x => !x.IsDeleted);
            int activeUserCount = await deviceData.Where(x => x.Status == DeviceStatus.Active).CountAsync();
            int inActiveUserCount = await deviceData.Where(x => x.Status == DeviceStatus.Inactive).CountAsync();
            int AndroidCount = await download.Where(x => !x.IsDeleted).SumAsync(x => x.AndroidCount);
            int IPhoneCount = await download.Where(x => !x.IsDeleted).SumAsync(x => x.IphoneCount);
            int NewMembers = await deviceData.Where(x => !x.IsDeleted && x.CreationDate.Month == DateTimeOffset.UtcNow.Month).CountAsync();

            var listDeviceData = await deviceData.ToListAsync();
            var activeUserMonthlyCount = listDeviceData.Where(data => data.Status == DeviceStatus.Active).GroupBy(data => data.CreationDate.Month)
                    .Select(group => new MonthCount
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(group.Key),
                        Count = group.Count()
                    })
                    .OrderByDescending(x => DateTime.ParseExact(x.Month, "MMMM", CultureInfo.CurrentCulture).Month)
                    .ToList();
            var inActiveUserMonthlyCount = listDeviceData.Where(data => data.Status == DeviceStatus.Inactive).GroupBy(data => data.CreationDate.Month)
                  .Select(group => new MonthCount
                  {
                      Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(group.Key),
                      Count = group.Count()
                  })
                  .OrderByDescending(x => DateTime.ParseExact(x.Month, "MMMM", CultureInfo.CurrentCulture).Month)
                  .ToList();
            var newMamberMonthlyCount = listDeviceData.Where(x => x.CreationDate.Month == DateTimeOffset.UtcNow.Month).GroupBy(data => data.CreationDate.Month)
                  .Select(group => new MonthCount
                  {
                      Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(group.Key),
                      Count = group.Count()
                  })
                  .OrderByDescending(x => DateTime.ParseExact(x.Month, "MMMM", CultureInfo.CurrentCulture).Month)
                  .ToList();

            return new AnalyticsViewModel()
            {
                ActiveUserCount = activeUserCount,
                InActiveUserCount = inActiveUserCount,
                AndroidCount = AndroidCount,
                IphoneCount = IPhoneCount,
                NewMemberCount = NewMembers,
                TotalPhoneCount = AndroidCount + IPhoneCount,
                ActiveUserMonthlyCount = activeUserMonthlyCount,
                InActiveUserMonthlyCount = inActiveUserMonthlyCount,
                NewMemberMonthlyCount = newMamberMonthlyCount
            };
        }
        public async Task<AnalyticsViewModel> GetAnalyTicsDetailsByYear(int year)
        {
            var deviceData = await _deviceDataRepository.GetAll().Where(x => !x.IsDeleted && x.CreationDate.Year == year).ToListAsync();
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            DateTimeFormatInfo dateTimeFormat = cultureInfo.DateTimeFormat;

            var activeUserMonthlyCount = deviceData.Where(data => data.Status == DeviceStatus.Active).GroupBy(data => data.CreationDate.Month)
                    .Select(group => new MonthCount
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(group.Key).Substring(0, 3),
                        Count = group.Count()
                    })
                    .OrderByDescending(x => DateTime.ParseExact(x.Month, "MMMM", CultureInfo.CurrentCulture).Month)
                    .ToList();

            var inActiveUserMonthlyCount = deviceData.Where(data => data.Status == DeviceStatus.Inactive).GroupBy(data => data.CreationDate.Month)
                  .Select(group => new MonthCount
                  {
                      Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(group.Key).Substring(0, 3),
                      Count = group.Count()
                  })
                  .OrderByDescending(x => DateTime.ParseExact(x.Month, "MMMM", CultureInfo.CurrentCulture).Month)
                  .ToList();
            var newMamberMonthlyCount = deviceData.Where(x => x.CreationDate.Month == DateTimeOffset.UtcNow.Month).GroupBy(data => data.CreationDate.Month)
                  .Select(group => new MonthCount
                  {
                      Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(group.Key).Substring(0, 3),
                      Count = group.Count()
                  })
                  .OrderByDescending(x => DateTime.ParseExact(x.Month, "MMMM", CultureInfo.CurrentCulture).Month)
                  .ToList();

            return new AnalyticsViewModel()
            {
                ActiveUserMonthlyCount = activeUserMonthlyCount,
                InActiveUserMonthlyCount = inActiveUserMonthlyCount,
                NewMemberMonthlyCount = newMamberMonthlyCount
            };
        }
    }
}
