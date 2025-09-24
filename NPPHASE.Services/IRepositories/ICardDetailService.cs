using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.IRepositories
{
    public interface ICardDetailService
    {
        Task<int> CreateUpdateCardDetails(CardDetail cardDetail);
        Task<PagedListViewModel<CardDetail>> GetAll(GetAllRequestViewModel model);
    }
}
