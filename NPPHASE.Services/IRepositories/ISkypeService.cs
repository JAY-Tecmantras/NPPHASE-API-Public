using NPPHASE.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.IRepositories
{
    public interface ISkypeService
    {
        Task<PagedListViewModel<SkypeViewModel>> GetAll(GetAllRequestViewModel model);
        Task<PagedListViewModel<SkypeViewModel>> GetSkypeByContactPersonName(GetAllRequestViewModel model);
        Task<bool> DeleteGroupData(GetDeviceIdAndNameViewModel model);
    }
}
