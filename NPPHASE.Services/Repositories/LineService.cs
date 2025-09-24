using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class LineService : ILineService
    {
        private readonly IService<Line> _service;
        public LineService(IService<Line> service)
        {
            _service = service;
        }
        public async Task<PagedListViewModel<LineViewModel>> GetAll(GetAllRequestViewModel model)
        {
            var fromdate = model.FromDate;
            model.FromDate = null;
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
           
            if (fromdate.HasValue && model.ToDate.HasValue)
                result = result.Where(t => t.MessageLogTime >= fromdate.Value.Date && t.MessageLogTime <= model.ToDate.Value.Date);
            
            var pagedResult = result.GroupBy(x => x.ContactPersonName).Select(t => new
            {
                key = t.Key,
                line = t.OrderByDescending(x => x.MessageLogTime).FirstOrDefault()
            });

            int totalCount = await pagedResult.CountAsync();
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                    .Take(model.PageSize.Value);
            }

            // Materialize pagedResult by calling ToListAsync() here
            var pagedResultList = await pagedResult.ToListAsync();

            var ListResponse = pagedResultList.Select(x => new LineViewModel()
            {
                ContactNumber = x.line.ContactNumber,
                ContactPersonName = x.line.ContactPersonName,
                DeviceUserId = x.line.DeviceUserId,
                Message = x.line.Message,
                MessageLogTime = x.line.MessageLogTime,
                LineId = x.line.LineId,
                MessageType = x.line.MessageType.Value.ToString(),
            }).OrderByDescending(x => x.MessageLogTime).ToList();
            return new PagedListViewModel<LineViewModel>
            {
                TotalCount = totalCount,
                ListResponse = ListResponse,
            };
        }

        public async Task<PagedListViewModel<LineViewModel>> GetLineByContactPersonName(GetAllRequestViewModel model)
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
            return new PagedListViewModel<LineViewModel>
            {
                TotalCount = await result.CountAsync(),
                ListResponse = await pagedResult.Select(x => new LineViewModel()
                {
                    ContactNumber = x.ContactNumber,
                    ContactPersonName = x.ContactPersonName,
                    DeviceUserId = x.DeviceUserId,
                    Message = x.Message,
                    MessageLogTime = x.MessageLogTime,
                    LineId = x.LineId,
                    MessageType = x.MessageType.Value.ToString(),
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
                                              model.SearchField.Contains(x.ContactPersonName))
                                  .AsNoTracking();

            if (!records.Any())
            {
                return false;
            }

            try
            {
                // Extract IDs for batch deletion
                var idsToDelete = records.Select(r => r.LineId).ToList();

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
