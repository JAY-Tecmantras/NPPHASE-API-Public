using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Implementations;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class CalendarService : ICalendarService
    {
        private readonly IService<Calendar> _service;
        public CalendarService(IService<Calendar> service)
        {
            _service = service;
        }
        public async Task<PagedListViewModel<Calendar>> GetAll(GetAllRequestViewModel model)
        {
            var fromdate = model.FromDate;
            model.FromDate = null;
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
            if (fromdate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.CalenderLogTime >= fromdate.Value && t.CalenderLogTime.Value.Date <= model.ToDate.Value);
            }
            result = result.OrderByDescending(x => x.CalenderLogTime);
            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }
            return new PagedListViewModel<Calendar>
            {
                TotalCount = result.Count(),
                ListResponse = await pagedResult.ToListAsync()
            };
        }
    }
}
