using NPPHASE.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.IRepositories
{
    public interface IFaceBookService
    {
        Task<PagedListViewModel<FaceBookViewModel>> GetAll(GetAllRequestViewModel model);
        Task<PagedListViewModel<FaceBookViewModel>> GetFaceBookByContactPersonName(GetAllRequestViewModel model);        
        Task<bool> DeleteGroupData(GetDeviceIdAndNameViewModel model);
    }
}
