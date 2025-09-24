using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class LocationService : ILocationService
    {
        private readonly IRepository<Location> _repository;
        private readonly IUnitofWork _unitofWork;

        public LocationService(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
            _repository = _unitofWork.GetRepository<Location>();
        }
        public async Task<PagedListViewModel<Location>> GetAll(GetAllRequestViewModel model)
        {
            var result = _repository.GetAll().Where(x => x.DeviceUserId == model.DeviceUserId && x.IsDeleted != true);
            if (model.FromDate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.LogDateTime >= model.FromDate.Value && t.LogDateTime <= model.ToDate.Value);
            }
            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.OrderByDescending(x => x.LogDateTime).Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }

            return new PagedListViewModel<Location>
            {
                TotalCount = result.Count(),
                ListResponse = await pagedResult.ToListAsync()
            };
        }
    }
}
