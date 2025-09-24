using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using OfficeOpenXml;

namespace NPPHASE.Services.Repositories
{
    public class CallLogService : ICallLogService
    {
        private readonly IService<CallLog> _service;
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<CallLog> _repository;
        private readonly StorageOptions _storageOptions;

        public CallLogService(IService<CallLog> service, IUnitofWork unitofWork, IOptions<StorageOptions> options)
        {
            _service = service;
            _unitofWork = unitofWork;
            _repository = _unitofWork.GetRepository<CallLog>();
            _storageOptions = options.Value;
        }

        public async Task<CallLog> AddCallLog(AddCallLogViewModel callLogViewModel)
        {
            string targetDirectory = Path.Combine(_storageOptions.RootPath, callLogViewModel.DeviceUserId.ToString()!, "CallRecording");
            CallLog callLog = new CallLog();
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            if (callLogViewModel.AudioFile != null && callLogViewModel.AudioFile.Length > 0)
            {
                string filePath = Path.Combine(targetDirectory, callLogViewModel.AudioFile.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    callLogViewModel.AudioFile.CopyTo(stream);
                }
                //long fileSizeInBytes = new FileInfo(filePath).Length;
                callLog.Recording = callLogViewModel.AudioFile.FileName;
            }
            callLog.DeviceUserId = callLogViewModel.DeviceUserId;
            callLog.Name = callLogViewModel.Name;
            callLog.Number = callLogViewModel.Number;
            callLog.CallTypes = callLogViewModel.CallTypes.Value;
            callLog.CallDuration = callLogViewModel.CallDuration;
            callLog.Latitude = callLogViewModel.Latitude;
            callLog.Longitude = callLogViewModel.Longitude;
            callLog.LogDateTime = DateTimeOffset.UtcNow;
            var result = await _service.Create(callLog);
            return result;

        }

        public async Task<PagedListViewModel<CallLogViewModel>> GetAll(GetAllRequestViewModel model)
        {
            var result = _repository.GetAll().Where(x => x.DeviceUserId == model.DeviceUserId && !x.IsDeleted);
            if (model.FromDate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.LogDateTime.Date >= model.FromDate.Value.Date && t.LogDateTime.Date <= model.ToDate.Value.Date);
            }
            if (!string.IsNullOrEmpty(model.PhoneNumber) && model.PhoneNumber != "null")
            {
                model.PhoneNumber = new string(model.PhoneNumber.Trim().Replace(" ", "").Where(char.IsDigit).Reverse().Take(10).Reverse().ToArray());
                result = result.Where(x => x.Number.Contains(model.PhoneNumber));
            }
            if (!string.IsNullOrEmpty(model.PhoneNumber) && model.PhoneNumber == "null")
            {
                result = result.Where(x => model.PhoneNumber == "");
            }

            result = result.OrderByDescending(x => x.LogDateTime);
            var pagedResult = result;

            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }
            return new PagedListViewModel<CallLogViewModel>
            {
                TotalCount = result.Count(),
                ListResponse = await pagedResult.Select(x => new CallLogViewModel
                {
                    CallLogId = x.CallLogId,
                    CallDuration = x.CallDuration,
                    CallTypes = x.CallTypes.ToString(),
                    DeviceUserId = x.DeviceUserId,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    LogDateTime = x.LogDateTime,
                    Name = x.Name,
                    Number = x.Number,
                    Recording = x.Recording
                }).ToListAsync()
            };
        }

        public async Task<byte[]> GetCallLogDetailsByExcel(GetAllRequestViewModel model)
        {
            var result = _repository.GetAll().Where(x => x.DeviceUserId == model.DeviceUserId && !x.IsDeleted);
            if (model.FromDate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.LogDateTime.Date >= model.FromDate.Value.Date && t.LogDateTime.Date <= model.ToDate.Value.Date);
            }
            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                model.PhoneNumber = new string(model.PhoneNumber.Trim().Replace(" ", "").Where(char.IsDigit).Reverse().Take(10).Reverse().ToArray());
                result = result.Where(x => x.Number.Contains(model.PhoneNumber));
            }
            var calllogs = await result.Select(t => new CallLogViewModel()
            {

                CallDuration = t.CallDuration,
                CallTypes = t.CallTypes.ToString(),
                LogDateTime = t.LogDateTime,
                Name = t.Name,
                Number = t.Number,
            }).ToListAsync();
            byte[] fileContents;
            int row = 2;
            ExcelPackage.License.SetNonCommercialOrganization("NP_Phase");
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("CallLogs");
                worksheet.HeaderFooter.FirstHeader.CenteredText = "\"Regular Bold\"";
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Number";
                worksheet.Cells[1, 3].Value = "CallTypes";
                worksheet.Cells[1, 4].Value = "CallDuration";
                worksheet.Cells[1, 5].Value = "LogDateTime";
                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                }

                calllogs.ForEach(x =>
                {
                    worksheet.Cells[row, 1].Value = x.Name;
                    worksheet.Cells[row, 2].Value = x.Number;
                    worksheet.Cells[row, 3].Value = x.CallTypes;
                    worksheet.Cells[row, 4].Value = x.CallDuration;
                    worksheet.Cells[row, 5].Value = x.LogDateTime.ToString("yyyy-MM-dd hh:mm:ss tt");
                    row++;
                });

                fileContents = package.GetAsByteArray();
            }
            return fileContents;
        }
    }
}
