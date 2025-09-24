using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class InternetHistoryService : IInternetHistoryService
    {
        private readonly IService<InternetHistory> _service;
        public InternetHistoryService(IService<InternetHistory> service)
        {
            _service = service;
        }
        public async Task<PagedListViewModel<InternetHistoryViewModel>> GetAll(GetAllRequestViewModel model)
        {
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
            result = result.OrderByDescending(x => x.WebLogTime);
            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }
            return new PagedListViewModel<InternetHistoryViewModel>
            {
                TotalCount = result.Count(),
                ListResponse = await pagedResult.Select(x => new InternetHistoryViewModel
                {
                    DeviceUserId = x.DeviceUserId,
                    InternetHistoryId = x.InternetHistoryId,
                    WebLogTime = x.WebLogTime,
                    WebUrl = x.WebUrl
                }).ToListAsync()
            };
        }
    }
}
