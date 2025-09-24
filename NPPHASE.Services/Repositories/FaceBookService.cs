using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Implementations;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class FaceBookService : IFaceBookService
    {
        private readonly IService<Facebook> _service;
        public FaceBookService(IService<Facebook> service)
        {
            _service = service;
       
        }
        public async Task<PagedListViewModel<FaceBookViewModel>> GetAll(GetAllRequestViewModel model)
        {
            var fromdate = model.FromDate;
            model.FromDate = null;
            var result = _service.GetAllDataAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
            if (fromdate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.MessageLogTime >= fromdate.Value && t.MessageLogTime <= model.ToDate.Value);
            }
            result = result.OrderByDescending(x => x.MessageLogTime);

            var groupedResult = result.GroupBy(x => x.ContactPersonName).Select(t => new
            {
                key = t.Key,
                facebook = t.OrderByDescending(x => x.MessageLogTime).FirstOrDefault()
            });

            int totalCount = await groupedResult.CountAsync();

            var pagedResult = groupedResult
                    .ToList()
                    .OrderByDescending(x => x.facebook?.MessageLogTime)
                    .AsQueryable();

            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                    .Take(model.PageSize.Value);
            }

            // Materialize pagedResult by calling ToListAsync() here
            var pagedResultList = pagedResult.ToList();

            var ListResponse = pagedResultList.Select(x => new FaceBookViewModel()
            {
                ContactNumber = x.facebook.ContactNumber,
                ContactPersonName = x.facebook.ContactPersonName,
                DeviceUserId = x.facebook.DeviceUserId,
                Message = x.facebook.Message,
                MessageLogTime = x.facebook.MessageLogTime,
                FacebookId = x.facebook.FacebookId,
                MessageType = x.facebook.MessageType.Value.ToString(),
                UserName = x.facebook.UserName
            }).OrderByDescending(x => x.MessageLogTime).ToList();
            return new PagedListViewModel<FaceBookViewModel>
            {
                TotalCount = totalCount,
                ListResponse = ListResponse,
            };
        }

        public async Task<PagedListViewModel<FaceBookViewModel>> GetFaceBookByContactPersonName(GetAllRequestViewModel model)
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
            return new PagedListViewModel<FaceBookViewModel>
            {
                TotalCount = await result.CountAsync(),
                ListResponse = await pagedResult.Select(x => new FaceBookViewModel
                {
                    ContactNumber = x.ContactNumber,
                    ContactPersonName = x.ContactPersonName,
                    DeviceUserId = x.DeviceUserId,
                    Message = x.Message,
                    MessageLogTime = x.MessageLogTime,
                    FacebookId = x.FacebookId,
                    MessageType = x.MessageType.Value.ToString(),
                    UserName = x.UserName
                }).ToListAsync()
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
                var idsToDelete = records.Select(r => r.FacebookId).ToList();

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
