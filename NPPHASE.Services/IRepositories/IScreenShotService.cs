using Microsoft.AspNetCore.Http;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.IRepositories
{
    public interface IScreenShotService
    {
        Task<ScreenShot> AddScreenShot(IFormFile formFiles, int deviceUserId);
        Task<PagedListViewModel<ScreenShotViewModel>> GetAll(GetAllRequestViewModel model);
    }
}
