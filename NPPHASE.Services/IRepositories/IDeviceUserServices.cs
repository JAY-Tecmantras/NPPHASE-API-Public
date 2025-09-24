using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.IRepositories
{
    public interface IDeviceUserServices
    {
        Task<DeviceUserViewModel> GetDeviceById(int id);
        Task<DeviceUser> GetDeviceUserDetails(string userId, string iMEINumber);
        Task<PagedListViewModel<DeviceUser>> GetAll(GetAllRequestViewModel model);
        Task<ResponseMessageViewModel> UpdateDeviceToken(UpdateDeviceTokenViewModel viewModel);
        Task<ResponseMessageViewModel> GetDeviceToken(int deviceUserId);
        int? GetDeviceUserIdByImemiNumber(string iMEINUmber);
        Task<bool> ResetAlacProgressColumn(int deviceUserId);
    }
}
