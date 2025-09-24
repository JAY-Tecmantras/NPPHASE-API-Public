using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.IRepositories
{
    public interface IInstalledAppService
    {
        Task<PagedListViewModel<InstalledAppViewModel>> GetAll(GetAllRequestViewModel model);
        Task<bool> RemoveInstalledApp(int deviceUserId);

    }
}
