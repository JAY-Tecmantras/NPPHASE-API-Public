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
    public class ScreenShotService : IScreenShotService
    {
        private readonly IService<ScreenShot> _service;
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<DeviceUser> _deviceUserRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StorageOptions _storageOptions;
        public ScreenShotService(IService<ScreenShot> service, IUnitofWork unitofWork, IHttpContextAccessor httpContextAccessor, IOptions<StorageOptions> options)
        {
            _service = service;
            _unitofWork = unitofWork;
            _deviceUserRepository = _unitofWork.GetRepository<DeviceUser>();
            _httpContextAccessor = httpContextAccessor;
            _storageOptions = options.Value;
        }
        public async Task<ScreenShot> AddScreenShot(IFormFile file, int deviceUserId)
        {
            string DeviceUniqueId = _deviceUserRepository.GetAll().Where(x => x.DeviceUserId == deviceUserId).FirstOrDefault().DeviceUniqueId;
            ScreenShot addScreenShot = new();
            string targetDirectory = Path.Combine(_storageOptions.RootPath, DeviceUniqueId, "ScreenShot");

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            if (file.Length > 0)
            {
                var fileName = $"{DateTime.UtcNow.ToString("ddMMyyyy_HHmmssff", CultureInfo.InvariantCulture)}_{DeviceUniqueId}_{file.FileName}";
                string filePath = Path.Combine(targetDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                long fileSizeInBytes = new FileInfo(filePath).Length;
                ScreenShot screenShot = new();
                screenShot.Name = fileName;
                screenShot.DeviceUserId = deviceUserId;
                screenShot.Size = Convert.ToString((double)fileSizeInBytes / (1024 * 1024));

                addScreenShot = await _service.Create(screenShot);
                _ = _unitofWork.commit();
            }
            return addScreenShot;
        }

        public async Task<PagedListViewModel<ScreenShotViewModel>> GetAll(GetAllRequestViewModel model)
        {
            string DeviceUniqueId = _deviceUserRepository.GetAll().Where(x => x.DeviceUserId == model.DeviceUserId && !x.IsDeleted).FirstOrDefault().DeviceUniqueId;

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
            var listResponse = await pagedResult.ToListAsync();
            return new PagedListViewModel<ScreenShotViewModel>
            {
                TotalCount = result.Count(),
                ListResponse = listResponse.Select(y => new ScreenShotViewModel()
                {
                    Id = y.Id,
                    DeviceUserId = y.DeviceUserId.Value,
                    FileUrl = string.Format("{0}/{1}/{2}/{3}/{4}", baseUrl, "Data", DeviceUniqueId, "ScreenShot", y.Name),
                    Name = y.Name,
                    Size = y.Size
                }).ToList()
            };
        }
    }
}
