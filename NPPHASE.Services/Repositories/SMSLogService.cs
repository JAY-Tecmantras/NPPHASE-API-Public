using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Implementations;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using OfficeOpenXml;

namespace NPPHASE.Services.Repositories
{
    public class SMSLogService : ISMSLogService
    {
        private readonly IService<SMSLog> _service;
        public SMSLogService(IService<SMSLog> service)
        {
            _service = service;
        }
        public async Task<PagedListViewModel<SmsViewModel>> GetAll(GetAllRequestViewModel model)
        {
            var fromdate = model.FromDate;
            model.FromDate = null;
            IQueryable<SMSLog> result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);

            if (fromdate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.LogDateTime.Date >= fromdate.Value.Date && t.LogDateTime.Date <= model.ToDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                result = result.Where(x => x.Number == model.PhoneNumber);
            }
            IQueryable<SMSLog> groupedResult;

            // If either email or message is empty, select the latest email for each group
            if (string.IsNullOrEmpty(model.PhoneNumber))
            {
                groupedResult = result.GroupBy(x => x.Number)
                                     .Select(g => g.OrderByDescending(x => x.LogDateTime).FirstOrDefault());
            }
            else
            {
                groupedResult = result;
            }
            var totalCount = groupedResult.Count();
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                groupedResult = groupedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                             .Take(model.PageSize.Value);
            }
            var listResponse = await groupedResult.ToListAsync();
            return new PagedListViewModel<SmsViewModel>
            {
                TotalCount = totalCount,
                ListResponse = listResponse.Select(x => new SmsViewModel()
                {
                    SMSLogId = x.SMSLogId,
                    DeviceUserId = x.DeviceUserId,
                    Name = x.Name,
                    Number = x.Number,
                    Message = x.Message,
                    SmsType = x.SmsType.ToString(),
                    LogDateTime = x.LogDateTime
                }).ToList(),
            };
        }

        public async Task<byte[]> GetSmsLogDetailsByExcel(GetAllRequestViewModel model)
        {
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId && !x.IsDeleted);
            if (model.FromDate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.LogDateTime.Date >= model.FromDate.Value.Date && t.LogDateTime.Date <= model.ToDate.Value.Date);
            }
            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                result = result.Where(x => x.Number.Contains(model.PhoneNumber));
            }
            var calllogs = await result.Select(t => new SmsViewModel()
            {

                Name = t.Name,
                Number = t.Number,
                Message = t.Message,
                SmsType = t.SmsType.ToString(),
                LogDateTime = t.LogDateTime,
            }).ToListAsync();
            byte[] fileContents;
            int row = 2;
            ExcelPackage.License.SetNonCommercialOrganization("NP_Phase");
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SmsLogs");
                worksheet.HeaderFooter.FirstHeader.CenteredText = "\"Regular Bold\"";
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Number";
                worksheet.Cells[1, 3].Value = "Message";
                worksheet.Cells[1, 4].Value = "SmsType";
                worksheet.Cells[1, 5].Value = "LogDateTime";
                using (var range = worksheet.Cells[1, 1, 1, 4])
                {
                    range.Style.Font.Bold = true;
                }

                calllogs.ForEach(x =>
                {
                    worksheet.Cells[row, 1].Value = x.Name;
                    worksheet.Cells[row, 2].Value = x.Number;
                    worksheet.Cells[row, 3].Value = x.Message;
                    worksheet.Cells[row, 4].Value = x.SmsType;
                    worksheet.Cells[row, 5].Value = x.LogDateTime.ToString("yyyy-MM-dd hh:mm:ss tt");
                    row++;
                });

                fileContents = package.GetAsByteArray();
            }
            return fileContents;
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
                                              model.SearchField.Contains(x.Number))
                                  .AsNoTracking();

            if (!records.Any())
            {
                return false;
            }

            try
            {
                // Extract IDs for batch deletion
                var idsToDelete = records.Select(r => r.SMSLogId).ToList();

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
