using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Enum;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class DownloadService : IDownloadService
    {
        private readonly IRepository<Download> repository;
        private readonly IUnitofWork _unitofWork;

        public DownloadService(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
            repository = _unitofWork.GetRepository<Download>();
        }
        public async Task<int> CreateUpdate(bool isAndroid)
        {
            Download download = new Download();
            var downloadData = await repository.GetAll().Where(x => x.Month == DateTimeOffset.UtcNow.Month &&
            x.Year == DateTimeOffset.Now.Year && !x.IsDeleted).FirstOrDefaultAsync();
            if (downloadData != null)
            {
                if (isAndroid == true)
                {
                    downloadData.AndroidCount = downloadData.AndroidCount + 1;
                }
                else
                {
                    downloadData.IphoneCount = downloadData.IphoneCount + 1;
                }
                repository.Update(downloadData);
            }
            else
            {
                download.Month = DateTimeOffset.UtcNow.Month;
                download.Year = DateTimeOffset.UtcNow.Year;
                download.AndroidCount = (isAndroid == true) ? 1 : 0;
                download.IphoneCount = (isAndroid == false) ? 1 : 0;
                await repository.Add(download);
            }
            return _unitofWork.commit();
        }
        public async Task<List<DownloadViewModel>> GetAllDownloadYearAndMonthWise(int year)
        {
            List<DownloadViewModel> downloadList = Enumerable.Range(1, 12)
                .Select(month => new DownloadViewModel { Month = Enum.GetName(typeof(Month), month) })
                .ToList();

            var result = await repository.GetAll().Where(x => !x.IsDeleted && x.Year == year).ToListAsync();

            if (result.Any())
            {
                foreach (var downloadViewModel in downloadList)
                {
                    var monthName = downloadViewModel.Month;

                    var matchingResult = result.FirstOrDefault(x => Enum.GetName(typeof(Month), x.Month) == monthName);

                    if (matchingResult != null)
                    {
                        downloadViewModel.AndroidCount = matchingResult.AndroidCount;
                        downloadViewModel.IphoneCount = matchingResult.IphoneCount;
                    }
                }
            }
            return downloadList;
        }

    }
}
