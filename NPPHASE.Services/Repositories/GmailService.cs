using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class GmailService : IGmailService
    {
        private readonly IService<Gmail> _service;
        public GmailService(IService<Gmail> service)
        {
            _service = service;
        }
        public async Task<PagedListViewModel<GmailViewModel>> GetAll(GetAllRequestViewModel model, string? email, string? message)
        {
            var fromdate = model.FromDate;
            model.FromDate = null;
            IQueryable<Gmail> result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);

            if (fromdate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.MailLogTime.Value.Date >= fromdate.Value.Date && t.MailLogTime.Value.Date <= model.ToDate.Value.Date);
            }
            if (!string.IsNullOrEmpty(email))
            {
                result = result.Where(x => x.FromEmail == email);
            }

            if (!string.IsNullOrEmpty(message))
            {
                result = result.Where(x => x.MessageBody.Contains(message));
            }

            IQueryable<Gmail> groupedResult;

            // If either email or message is empty, select the latest email for each group
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(message))
            {
                groupedResult = result.GroupBy(x => x.FromEmail)
                                     .Select(g => g.OrderByDescending(x => x.MailLogTime).FirstOrDefault());
            }
            else
            {
                groupedResult = result;
            }

            // Apply pagination if provided
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                groupedResult = groupedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                             .Take(model.PageSize.Value);
            }

            var ListResponse = await groupedResult.ToListAsync();

            return new PagedListViewModel<GmailViewModel>
            {
                TotalCount = result.Count(),
                ListResponse = ListResponse.Select(x => new GmailViewModel
                {
                    ToEmail = x.ToEmail,
                    Subject = x.Subject,
                    MessageBody = x.MessageBody,
                    MailType = x.MailType.ToString(),
                    MailLogTime = x.MailLogTime,
                    GmailId = x.GmailId,
                    FromEmail = x.FromEmail,
                    DeviceUserId = x.DeviceUserId,
                    BccEmail = x.BccEmail,
                    CcEmail = x.CcEmail
                }).ToList()
            };
        }
    }
}
