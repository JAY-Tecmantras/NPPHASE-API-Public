using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class SkypeService : ISkypeService
    {
        private readonly IService<Skype> _service;
        public SkypeService(IService<Skype> service)
        {
            _service = service;
        }
        public async Task<PagedListViewModel<SkypeViewModel>> GetAll(GetAllRequestViewModel model)
        {
            var fromdate = model.FromDate;
            model.FromDate = null;
            var result = _service.GetAllDataAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);

            if (fromdate.HasValue && model.ToDate.HasValue)
                result = result.Where(t => t.MessageLogTime >= fromdate.Value && t.MessageLogTime <= model.ToDate.Value);

            if (!string.IsNullOrEmpty(model.UserName))
            {
                result = result.Where(x => x.ContactPersonName == model.UserName);
            }
            var groupedResult = result.GroupBy(x => x.ContactPersonName).Select(t => new
            {
                key = t.Key,
                skype = t.OrderByDescending(x => x.MessageLogTime).FirstOrDefault()
            });
            
            int totalCount = await groupedResult.CountAsync();

            var pagedResult = groupedResult
                    .ToList()
                    .OrderByDescending(x => x.skype?.MessageLogTime)
                    .AsQueryable();

            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                    .Take(model.PageSize.Value);
            }

            // Materialize pagedResult by calling ToListAsync() here
            var pagedResultList = pagedResult.ToList();

            var ListResponse = pagedResultList.Select(x => new SkypeViewModel()
            {
                ContactNumber = x.skype.ContactNumber,
                ContactPersonName = x.skype.ContactPersonName,
                DeviceUserId = x.skype.DeviceUserId,
                Message = x.skype.Message,
                MessageLogTime = x.skype.MessageLogTime,
                SkypeId = x.skype.SkypeId,
                MessageType = x.skype.MessageType.Value.ToString(),
                UserName = x.skype.UserName
            }).OrderByDescending(x => x.MessageLogTime).ToList();
            return new PagedListViewModel<SkypeViewModel>
            {
                TotalCount = totalCount,
                ListResponse = ListResponse,
            };
        }

        public async Task<PagedListViewModel<SkypeViewModel>> GetSkypeByContactPersonName(GetAllRequestViewModel model)
        {
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
            if (!string.IsNullOrEmpty(model.ContactPersonName))
            {
                result = result.Where(x => x.ContactPersonName == model.ContactPersonName);
            }
            var pagedResult = result;

            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                    .Take(model.PageSize.Value);
            }
            return new PagedListViewModel<SkypeViewModel>
            {
                TotalCount = await result.CountAsync(),
                ListResponse = await pagedResult.Select(x => new SkypeViewModel()
                {
                    ContactNumber = x.ContactNumber,
                    ContactPersonName = x.ContactPersonName,
                    DeviceUserId = x.DeviceUserId,
                    Message = x.Message,
                    MessageLogTime = x.MessageLogTime,
                    SkypeId = x.SkypeId,
                    MessageType = x.MessageType.Value.ToString(),
                    UserName = x.UserName
                }).OrderByDescending(x => x.MessageLogTime).ToListAsync()
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
                                              model.SearchField.Contains(x.ContactPersonName))
                                  .AsNoTracking();

            if (!records.Any())
            {
                return false;
            }

            try
            {
                // Extract IDs for batch deletion
                var idsToDelete = records.Select(r => r.SkypeId).ToList();

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
