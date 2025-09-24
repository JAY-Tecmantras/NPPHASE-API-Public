using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.IRepositories
{
    public interface IKeyLoggerServices
    {
        Task<PagedListViewModel<KeyLogger>> GetAll(GetAllRequestViewModel model);
    }
}
