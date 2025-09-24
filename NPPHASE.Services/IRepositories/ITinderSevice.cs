using NPPHASE.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.IRepositories
{
    public interface ITinderSevice
    {
        Task<PagedListViewModel<TinderViewModel>> GetAll(GetAllRequestViewModel model);
        Task<PagedListViewModel<TinderViewModel>> GetTinderByUserName(GetAllRequestViewModel model);
        Task<bool> DeleteGroupData(GetDeviceIdAndNameViewModel model);
    }
}
