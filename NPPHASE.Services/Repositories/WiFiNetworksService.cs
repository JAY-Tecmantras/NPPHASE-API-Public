using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class WiFiNetworksService : IWiFiNetworksService
    {
        private readonly IService<WiFiNetwork> _service;

        public WiFiNetworksService(IService<WiFiNetwork> service)
        {
            _service = service;
        }
        public async Task<PagedListViewModel<WiFiNetworksViewModel>> GetAll(GetAllRequestViewModel model)
        {
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }
            return new PagedListViewModel<WiFiNetworksViewModel>
            {
                TotalCount = result.Count(),
                ListResponse = await pagedResult.Select(x => new WiFiNetworksViewModel
                {
                    DeviceUserId = x.DeviceUserId,
                    IsProtecteted = x.IsProtecteted,
                    WiFINetworkId = x.WiFINetworkId,
                    WiFINetworkName = x.WiFINetworkName,
                    Strength = x.Strength,
                    CreationDate = x.CreationDate
                }).ToListAsync()
            };
        }
    }
}
