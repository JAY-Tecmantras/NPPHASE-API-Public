using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NPPHASE.Data.Enum;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using System.Globalization;
using System.IO.Compression;

namespace NPPHASE.Services.Repositories
{
    public class GalleryService : IGalleryService
    {
        private readonly IRepository<Gallery> _repository;
        private readonly IService<Gallery> _service;
        private readonly IRepository<DeviceUser> _deviceUserRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitofWork _unitofWork;
        private readonly StorageOptions _storageOptions;
        public GalleryService(IUnitofWork unitofWork, IService<Gallery> service, IHttpContextAccessor httpContextAccessor, IOptions<StorageOptions> options)
        {
            _unitofWork = unitofWork;
            _repository = _unitofWork.GetRepository<Gallery>();
            _service = service;
            _deviceUserRepository = _unitofWork.GetRepository<DeviceUser>();
            _httpContextAccessor = httpContextAccessor;
            _storageOptions = options.Value;
        }

        public async Task<Gallery> AddGallery(IFormFile file, int deviceUserId)
        {

            int result = 0;
            string DeviceUniqueId = _deviceUserRepository.GetAll().Where(x => x.DeviceUserId == deviceUserId).FirstOrDefault().DeviceUniqueId;
            Gallery addGallery = new Gallery();
            string targetDirectory = Path.Combine(_storageOptions.RootPath, DeviceUniqueId, "Gallery");

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
                Gallery gallery = new Gallery();
                gallery.Name = fileName;
                gallery.LogDateTime = DateTimeOffset.UtcNow;
                gallery.DeviceUserId = deviceUserId;
                gallery.Size = Convert.ToString((double)fileSizeInBytes / (1024 * 1024));

                var contentType = GetMimeType(file.FileName);
                if (contentType.StartsWith("image/"))
                {
                    gallery.FileTypes = FileType.Image;
                }
                else if (contentType.StartsWith("video/"))
                {
                    gallery.FileTypes = FileType.Video;
                }
                addGallery = await _service.Create(gallery);
                result = _unitofWork.commit();

            }

            return addGallery;
        }

        public async Task<PagedListViewModel<GalleryViewModel>> GetAll(GetAllRequestViewModel model)
        {
            string DeviceUniqueId = _deviceUserRepository.GetAll().Where(x => x.DeviceUserId == model.DeviceUserId && !x.IsDeleted).FirstOrDefault().DeviceUniqueId;

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var result = _repository.GetAll().Where(x => x.DeviceUserId == model.DeviceUserId);
            if (model.FromDate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.LogDateTime.Date >= model.FromDate.Value.Date && t.LogDateTime.Date <= model.ToDate.Value.Date);
            }
            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }
            var listResponse = await pagedResult.ToListAsync();
            return new PagedListViewModel<GalleryViewModel>
            {
                TotalCount = result.Count(),
                ListResponse = listResponse.Select(y => new GalleryViewModel()
                {
                    GalleryId = y.GalleryId,
                    DeviceUserId = y.DeviceUserId,
                    FileTypes = y.FileTypes.ToString(),
                    LogDateTime = y.LogDateTime,
                    FileUrl = string.Format("{0}/{1}/{2}/{3}/{4}", baseUrl, "Data", DeviceUniqueId, "Gallery", y.Name),
                    Name = y.Name,
                    Size = y.Size
                }).ToList()
            };
        }

        public async Task<PagedListViewModel<GalleryViewModel>> GetAllGalleryTypeWise(GetAllRequestViewModel model, FileType? FileType)
        {
            string DeviceUniqueId = _deviceUserRepository.GetAll().Where(x => x.DeviceUserId == model.DeviceUserId).FirstOrDefault().DeviceUniqueId;

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var result = _repository.GetAll().Where(x => !x.IsDeleted && x.DeviceUserId == model.DeviceUserId && (FileType == null || x.FileTypes == FileType));

            if (model.FromDate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.LogDateTime.Date >= model.FromDate.Value.Date && t.LogDateTime.Date <= model.ToDate.Value.Date);
            }
            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }
            var listResponse = await pagedResult.ToListAsync();
            return new PagedListViewModel<GalleryViewModel>
            {
                TotalCount = result.Count(),
                ListResponse = listResponse.Select(y => new GalleryViewModel()
                {
                    GalleryId = y.GalleryId,
                    DeviceUserId = y.DeviceUserId,
                    FileTypes = y.FileTypes.ToString(),
                    LogDateTime = y.LogDateTime,
                    FileUrl = string.Format("{0}/{1}/{2}/{3}/{4}", baseUrl, "Data", DeviceUniqueId, "Gallery", y.Name),
                    Name = y.Name,
                    Size = y.Size
                }).ToList()
            };
        }


        public async Task<byte[]> DownloadZipAsync(List<int> galleryIds)
        {
            var galleries = _repository.GetAll().Where(x => galleryIds.Contains(x.GalleryId)).Select(x => new
            {
                x.Name,
                x.DeviceUser.DeviceUniqueId
            }).ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var gallery in galleries)
                    {
                        string deviceUniqueId = gallery.DeviceUniqueId;
                        string filePath = Path.Combine(_storageOptions.RootPath, deviceUniqueId, "Gallery", gallery.Name);

                        if (File.Exists(filePath))
                        {
                            var entry = archive.CreateEntry(gallery.Name, CompressionLevel.Fastest);

                            using (var entryStream = entry.Open())
                            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                    }
                }
                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> test(List<int> galleryIds)
        {
            var galleries = _repository.GetAll().Where(x => galleryIds.Contains(x.GalleryId)).Select(x => new
            {
                x.Name,
                x.DeviceUser.DeviceUniqueId
            }).ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var gallery in galleries)
                    {
                        string deviceUniqueId = gallery.DeviceUniqueId;
                        string filePath = Path.Combine(_storageOptions.RootPath, deviceUniqueId, "Gallery", gallery.Name);

                        if (File.Exists(filePath))
                        {
                            var entry = archive.CreateEntry(gallery.Name, CompressionLevel.Fastest);

                            using (var entryStream = entry.Open())
                            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                    }
                }
                return memoryStream.ToArray();
            }
        }


        #region Private Method
        private string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
        #endregion
    }
}
