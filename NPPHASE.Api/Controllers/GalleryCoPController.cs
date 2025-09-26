using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Enum;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using System.IO.Compression;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class GalleryCoPController : BaseController<Gallery>
    {
        private readonly IService<Gallery> _service;
        private readonly IActionResult _result;
        private readonly ILogger _logger;
        private readonly IGalleryService _galleryService;

        public GalleryCoPController(IService<Gallery> service, IGalleryService galleryService, IDeviceUserServices deviceUserServices) : base(service, deviceUserServices)
        {
            _service = service;
            _galleryService = galleryService;
        }
        public async override Task<IActionResult> GetAll([FromQuery] GetAllRequestViewModel model)
        {
            return new OkObjectResult(new ResponseMessageViewModel { Data = await _galleryService.GetAll(model), IsSuccess = true });
        }
        [HttpGet("GetAllGalleryTypeWise")]
        public async Task<IActionResult> GetAllGalleryTypeWise([FromQuery] GetAllRequestViewModel model, FileType? FileType)
        {
            var result = await _galleryService.GetAllGalleryTypeWise(model, FileType);
            return new OkObjectResult(new ResponseMessageViewModel { Data = result, IsSuccess = true });
        }
        [HttpPost("AddGallery")]
        public async Task<IActionResult> AddGallery(IFormFile files, [FromQuery] int deviceUserId)
        {
            var result = await _galleryService.AddGallery(files, deviceUserId);
            return new OkObjectResult(new ResponseMessageViewModel { Data = result, IsSuccess = true });
        }

        [HttpPost("DownloadZip")]
        public async Task<IActionResult> DownloadZip([FromBody] List<int> galleryId)
        {
            if (galleryId?.Count == 0)
                return BadRequest(new ResponseMessageViewModel { IsSuccess = false, Message = "No gallery ids provided" });

            var zipFile = await _galleryService.DownloadZipAsync(galleryId);

            return File(zipFile, "application/zip", $"Gallery_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip");
        }
    }
}
