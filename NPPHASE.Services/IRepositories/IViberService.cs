using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.IRepositories
{
    public interface IViberService
    {
        Task<PagedListViewModel<ViberViewModel>> GetAll(GetAllRequestViewModel model);
        Task<PagedListViewModel<ViberViewModel>> GetViberByContactPersonName(GetAllRequestViewModel model);
        Task<bool> DeleteGroupData(GetDeviceIdAndNameViewModel model);
    }
}
