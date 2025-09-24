using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class ScreenTimeService : IScreenTimeService
    {
        private readonly IService<ScreenTime> _service;
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<ScreenTime> _repository;

        public ScreenTimeService(IService<ScreenTime> service, IUnitofWork unitofWork)
        {
            _service = service;
            _unitofWork = unitofWork;
            _repository = _unitofWork.GetRepository<ScreenTime>();
        }

        public async Task<int> CreateUpdateScreenTime(ScreenTimeViewModel model)
        {
            var updateScreenTime = await _repository.GetAll().Where(x => !x.IsDeleted && x.AppName == model.AppName && x.CreationDate.Date == DateTimeOffset.Now.Date).FirstOrDefaultAsync();
            if (updateScreenTime != null)
            {
                updateScreenTime.ScreenTimeDuration = model.ScreenTimeDuration;
                _repository.Update(updateScreenTime);
            }
            else
            {
                ScreenTime screenTime = new ScreenTime();
                screenTime.AppName = model.AppName;
                screenTime.DeviceUserId = model.DeviceUserId;
                screenTime.ScreenTimeDuration = model.ScreenTimeDuration;
                await _repository.Add(screenTime);
            }
            return _unitofWork.commit();
        }

        public async Task<PagedListViewModel<ScreenTimeViewModel>> GetAll(GetAllRequestViewModel model)
        {
            try
            {
                var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
                result = result.OrderByDescending(x => x.CreationDate)
                               .ThenByDescending(x => !string.IsNullOrWhiteSpace(x.ScreenTimeDuration.Trim()) ? Convert.ToInt64(x.ScreenTimeDuration) : 0);
                var pagedResult = result;
                if (model.Page.HasValue && model.PageSize.HasValue)
                {
                    pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                            .Take(model.PageSize.Value);
                }
                var listRespons = await pagedResult.Select(x => new
                {
                    AppName = x.AppName,
                    DeviceUserId = x.DeviceUserId,
                    ScreenTimeId = x.ScreenTimeId,
                    ScreenTimeDuration = !string.IsNullOrEmpty(x.ScreenTimeDuration.Trim()) ? Convert.ToInt64(x.ScreenTimeDuration) : 0,
                    CreationDate = x.CreationDate,
                }).ToListAsync();
               // string topAppName = string.Join(",", listRespons.OrderByDescending(x => x.ScreenTimeDuration).Select(y => y.AppName).Take(5));
                return new PagedListViewModel<ScreenTimeViewModel>
                {
                    TotalCount = result.Count(),
                    ListResponse = listRespons.Select(x => new ScreenTimeViewModel
                    {
                        AppName = x.AppName,
                        DeviceUserId = x.DeviceUserId,
                        ScreenTimeDuration = Convert.ToString(listRespons.Where(sc => sc.ScreenTimeId == x.ScreenTimeId).Select(t => t.ScreenTimeDuration).Sum()),
                        ScreenTimeId = x.ScreenTimeId,
                     //   TopApplicationName = topAppName,
                        //TotalScreenTime = Convert.ToString(listRespons.Select(y => y.ScreenTimeDuration).Sum()),
                        CreationDate = x.CreationDate
                    }).OrderByDescending(x=> Convert.ToDouble(x.ScreenTimeDuration)).ToList()
                };
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<ScreenTimeAppNameTotalTimeViewModel> Top5AppAndScreenTime(GetAllRequestViewModel model)
        {
            ScreenTimeAppNameTotalTimeViewModel viewModel = new ScreenTimeAppNameTotalTimeViewModel();
            var listScreenTime = await _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId)
                .OrderByDescending(x => x.ScreenTimeDuration).Select(screenTime => new AppNameScreenTimeViewModel
                {
                    AppName = screenTime.AppName,
                    ScreenTimeDuration = !string.IsNullOrEmpty(screenTime.ScreenTimeDuration.Trim()) ? Convert.ToInt64(screenTime.ScreenTimeDuration) : 0,
                }).ToListAsync();
            TimeSpan TotalTime =(TimeSpan)(model.ToDate - model.FromDate);
            double totalMinutes = TotalTime.TotalMinutes;
            viewModel.AppName = listScreenTime.Select(y => y.AppName).Take(5).ToList();
            viewModel.TotalTime = String.Format("{0:0.00}", totalMinutes);
            viewModel.TotalScreenTime = Convert.ToString(listScreenTime.Select(y => y.ScreenTimeDuration).Sum());
            return viewModel;
        }
    }
}
