using NPPHASE.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.IRepositories
{
    public interface IWhatsappService
    {
        Task<PagedListViewModel<WhatsappViewModel>> GetAll(GetAllRequestViewModel model);
        Task<PagedListViewModel<WhatsappViewModel>> GetWhatsAppByPhoneNumber(GetAllRequestViewModel model);
        Task<bool> DeleteGroupData(GetDeviceIdAndNameViewModel model);
    }
}
