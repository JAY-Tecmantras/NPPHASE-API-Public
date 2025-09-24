using NPPHASE.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.IRepositories
{
    public interface IInternetHistoryService
    {
        Task<PagedListViewModel<InternetHistoryViewModel>> GetAll(GetAllRequestViewModel model);

    }
}
