using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class TinderSevice : ITinderSevice
    {
        private readonly IService<Tinder> _service;

        public TinderSevice(IService<Tinder> service)
        {
            _service = service;
        }
        public async Task<PagedListViewModel<TinderViewModel>> GetAll(GetAllRequestViewModel model)
        {
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
            var pagedResult = result.GroupBy(x => x.ContactNumber).Select(t => new
            {
                key = t.Key,
                tinder = t.OrderByDescending(x => x.MessageLogTime).FirstOrDefault()
            });
            int totalCount = await pagedResult.CountAsync();
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                    .Take(model.PageSize.Value);
            }

            // Materialize pagedResult by calling ToListAsync() here
            var pagedResultList = await pagedResult.ToListAsync();

            var ListResponse = pagedResultList.Select(x => new TinderViewModel()
            {
                ContactNumber = x.tinder.ContactNumber,
                ContactPersonName = x.tinder.ContactPersonName,
                DeviceUserId = x.tinder.DeviceUserId,
                Message = x.tinder.Message,
                MessageLogTime = x.tinder.MessageLogTime,
                TinderId = x.tinder.TinderId,
                MessageType = x.tinder.MessageType.Value.ToString(),
                UserName = x.tinder.UserName
            }).OrderByDescending(x => x.MessageLogTime).ToList();
            return new PagedListViewModel<TinderViewModel>
            {
                TotalCount = totalCount,
                ListResponse = ListResponse,
            };
        }

        public async Task<PagedListViewModel<TinderViewModel>> GetTinderByUserName(GetAllRequestViewModel model)
        {
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId && (string.IsNullOrEmpty(model.UserName) || x.UserName == model.UserName));
            if (!string.IsNullOrEmpty(model.UserName))
            {
                result = result.Where(x => x.UserName == model.UserName);
            }
            var pagedResult = result;

            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                    .Take(model.PageSize.Value);
            }
            return new PagedListViewModel<TinderViewModel>
            {
                TotalCount = await result.CountAsync(),
                ListResponse = await pagedResult.Select(x => new TinderViewModel()
                {
                    ContactNumber = x.ContactNumber,
                    ContactPersonName = x.ContactPersonName,
                    DeviceUserId = x.DeviceUserId,
                    Message = x.Message,
                    MessageLogTime = x.MessageLogTime,
                    TinderId = x.TinderId,
                    MessageType = x.MessageType.Value.ToString(),
                    UserName = x.UserName
                }).OrderByDescending(x => x.MessageLogTime).ToListAsync(),
            };
        }

        public async Task<bool> DeleteGroupData(GetDeviceIdAndNameViewModel model)
        {
            // Validate input
            if (model.DeviceUserId == null || model.SearchField == null || !model.SearchField.Any())
            {
                return false;
            }

            // Fetch matching records
            var records = _service.GetAllAsync(new GetAllRequestViewModel())
                                  .Where(x => x.DeviceUserId == model.DeviceUserId &&
                                              model.SearchField.Contains(x.UserName))
                                  .AsNoTracking();

            if (!records.Any())
            {
                return false;
            }

            try
            {
                // Extract IDs for batch deletion
                var idsToDelete = records.Select(r => r.TinderId).ToList();

                // Call DeleteRange with the list of IDs
                bool isDeleted = await _service.DeleteRange(idsToDelete);

                return isDeleted;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
