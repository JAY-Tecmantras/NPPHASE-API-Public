using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class KeyLoggerServices : IKeyLoggerServices
    {
        private readonly IService<KeyLogger> _service;
        public KeyLoggerServices(IService<KeyLogger> service)
        {
            _service = service;
        }
        public async Task<PagedListViewModel<KeyLogger>> GetAll(GetAllRequestViewModel model)
        {
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
            result = result.OrderByDescending(x => x.LogTime);
            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }
            return new PagedListViewModel<KeyLogger>
            {
                TotalCount = result.Count(),
                ListResponse = await pagedResult.Select(x => new KeyLogger
                {
                    KeyLoggerId = x.KeyLoggerId,
                    DeviceUserId = x.DeviceUserId,
                    AppName = x.AppName,
                    KeyValue = x.KeyValue,
                    LogTime = x.LogTime,
                }).ToListAsync()
            };
        }
    }
}
