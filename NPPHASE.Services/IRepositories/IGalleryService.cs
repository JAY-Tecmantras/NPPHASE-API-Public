using Microsoft.AspNetCore.Http;
using NPPHASE.Data.Enum;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;

namespace NPPHASE.Services.IRepositories
{
    public interface IGalleryService
    {
        Task<PagedListViewModel<GalleryViewModel>> GetAllGalleryTypeWise(GetAllRequestViewModel model, FileType? FileType);
        Task<PagedListViewModel<GalleryViewModel>> GetAll(GetAllRequestViewModel model);
        Task<Gallery> AddGallery(IFormFile formFiles, int deviceUserId);
        Task<byte[]> DownloadZipAsync(List<int> galleryIds);
        Task<byte[]> test(List<int> galleryIds);
    }
}
