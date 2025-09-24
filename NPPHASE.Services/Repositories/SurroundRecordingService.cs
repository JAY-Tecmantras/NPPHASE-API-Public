using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using System.Globalization;

namespace NPPHASE.Services.Repositories
{
    public class SurroundRecordingService : ISurroundRecordingService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IService<SurroundRecordings> _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StorageOptions _storageOptions;

        public SurroundRecordingService(IUnitofWork unitofWork, IService<SurroundRecordings> service, IHttpContextAccessor httpContextAccessor, IOptions<StorageOptions> options)
        {
            _unitofWork = unitofWork;
            _service = service;
            _httpContextAccessor = httpContextAccessor;
            _storageOptions = options.Value;


        }
        public async Task<SurroundRecordings> AddSurroundRecording(SurroundRecordingViewModel addSurroundRecording)
        {
            SurroundRecordings surroundRecordings = new();
            SurroundRecordings getsurroundRecordings = new();
            string targetDirectory = Path.Combine(_storageOptions.RootPath, addSurroundRecording.DeviceUserId.ToString(), "SurroundRecording");

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            if (addSurroundRecording.File != null && addSurroundRecording.File.Length > 0)
            {
                var fileName = $"{DateTime.UtcNow.ToString("ddMMyyyy_HHmmssff", CultureInfo.InvariantCulture)}_{addSurroundRecording.DeviceUserId}_{addSurroundRecording.File.FileName}";
                string filePath = Path.Combine(targetDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    addSurroundRecording.File.CopyTo(stream);
                }
                long fileSizeInBytes = new FileInfo(filePath).Length;
                surroundRecordings.RecordingName = fileName;
                surroundRecordings.DeviceUserId = addSurroundRecording.DeviceUserId.Value;
                surroundRecordings.RecordingDuration = addSurroundRecording.Duration;
                getsurroundRecordings = await _service.Create(surroundRecordings);
                _ = _unitofWork.commit();

            }
            return getsurroundRecordings;
        }

        public async Task<PagedListViewModel<SurroundRecordingViewModel>> GetAll(GetAllRequestViewModel model)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var result = _service.GetAllAsync(model);
            result = result.Where(x => x.DeviceUserId == model.DeviceUserId);

            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }
            return new PagedListViewModel<SurroundRecordingViewModel>
            {
                TotalCount = result.Count(),
                ListResponse = await pagedResult.Select(y => new SurroundRecordingViewModel()
                {
                    Id = y.Id,
                    Name = y.RecordingName,
                    Duration = y.RecordingDuration,
                    FileUrl = string.Format("{0}/{1}/{2}/{3}/{4}", baseUrl, "Data", model.DeviceUserId, "SurroundRecording", y.RecordingName),
                    LogDateTime = y.CreationDate
                }).ToListAsync()
            };
        }

    }
}
