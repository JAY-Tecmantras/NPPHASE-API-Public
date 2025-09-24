using NPPHASE.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.IRepositories
{
    public interface IKikService
    {
        Task<PagedListViewModel<KikViewModel>> GetAll(GetAllRequestViewModel model);
        Task<PagedListViewModel<KikViewModel>> GetKikByContactPersonName(GetAllRequestViewModel model);
        Task<bool> DeleteGroupData(GetDeviceIdAndNameViewModel model);
    }
}
