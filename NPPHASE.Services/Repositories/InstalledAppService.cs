using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class InstalledAppService : IInstalledAppService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<InstalledApp> _repository;

        public InstalledAppService(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
            _repository = _unitofWork.GetRepository<InstalledApp>();

        }
        public async Task<PagedListViewModel<InstalledAppViewModel>> GetAll(GetAllRequestViewModel model)
        {
            var result = _repository.GetAll().Where(x => x.DeviceUserId == model.DeviceUserId && x.IsDeleted != true);
            if (model.FromDate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.LogDateTime.Value.Date >= model.FromDate.Value.Date && t.LogDateTime.Value.Date <= model.ToDate.Value.Date);
            }

            result = result.OrderByDescending(x => x.LogDateTime);
            var pagedResult = result;

            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }
            return new PagedListViewModel<InstalledAppViewModel>
            {
                TotalCount = result.Count(),
                ListResponse = await pagedResult.Select(x => new InstalledAppViewModel
                {
                    AppSize = x.AppSize,
                    DeviceUserId = x.DeviceUserId,
                    InstalledAppId = x.InstalledAppId,
                    InstalledAppName = x.InstalledAppName,
                    LogDateTime = x.LogDateTime
                }).ToListAsync()
            };
        }

        public async Task<bool> RemoveInstalledApp(int deviceUserId)
        {
            var result = false;
            var InstalledApp = await _repository.GetAll().Where(x => x.DeviceUserId == deviceUserId && !x.IsDeleted).ToListAsync();
            if (InstalledApp.Any())
            {
                _repository.RemoveRange(InstalledApp);
                if (_unitofWork.commit() == 1)
                {
                    result = true;
                }
            }
            else
            {
                result = true;
            }
            return result;
        }
    }
}
