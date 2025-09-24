using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class WhatsappService : IWhatsappService
    {
        private readonly IService<Whatsapp> _service;
        public WhatsappService(IService<Whatsapp> service)
        {
            _service = service;
        }
        public async Task<PagedListViewModel<WhatsappViewModel>> GetAll(GetAllRequestViewModel model)
        {
            var fromdate = model.FromDate;
            model.FromDate = null;
            var result = _service.GetAllDataAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
            if (fromdate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.MessageLogTime >= fromdate.Value && t.MessageLogTime <= model.ToDate.Value);
            }
            var groupedResult = result.GroupBy(x => x.ContactPersonName).Select(t => new
            {
                key = t.Key,
                whatsapp = t.OrderByDescending(x => x.MessageLogTime).FirstOrDefault()
            });
            var countPagedResult = await groupedResult.CountAsync();

            var pagedResult = groupedResult
                    .ToList()
                    .OrderByDescending(x => x.whatsapp?.MessageLogTime)
                    .AsQueryable();

            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                    .Take(model.PageSize.Value);
            }

            // Materialize pagedResult by calling ToListAsync() here
            var pagedResultList = pagedResult.ToList();

            var ListResponse = pagedResultList.Select(x => new WhatsappViewModel()
            {
                ContactNumber = x.whatsapp?.ContactNumber,
                ContactPersonName = x.whatsapp?.ContactPersonName,
                DeviceUserId = x.whatsapp?.DeviceUserId,
                Message = x.whatsapp?.Message,
                MessageLogTime = x.whatsapp?.MessageLogTime,
                WhatsappId = x.whatsapp.WhatsappId,
                MessageType = Convert.ToString(x.whatsapp?.MessageType),
            }).OrderByDescending(x => x.MessageLogTime).ToList();

            return new PagedListViewModel<WhatsappViewModel>
            {
                TotalCount = countPagedResult,
                ListResponse = ListResponse,
            };

        }

        public async Task<PagedListViewModel<WhatsappViewModel>> GetWhatsAppByPhoneNumber(GetAllRequestViewModel model)
        {
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
            if (!string.IsNullOrEmpty(model.UserName))
            {
                result = result.Where(x => x.ContactPersonName.Contains(model.UserName));
            }
            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                    .Take(model.PageSize.Value);
            }
            return new PagedListViewModel<WhatsappViewModel>
            {
                TotalCount = await result.CountAsync(),
                ListResponse = await pagedResult.Select(x => new WhatsappViewModel()
                {
                    ContactNumber = x.ContactNumber,
                    ContactPersonName = x.ContactPersonName,
                    DeviceUserId = x.DeviceUserId,
                    Message = x.Message,
                    MessageLogTime = x.MessageLogTime,
                    WhatsappId = x.WhatsappId,
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
                var idsToDelete = records.Select(r => r.WhatsappId).ToList();

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
