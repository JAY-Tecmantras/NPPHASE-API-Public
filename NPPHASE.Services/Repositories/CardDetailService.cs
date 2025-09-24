using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class CardDetailService : ICardDetailService
    {
        private readonly IRepository<CardDetail> _repository;
        private readonly IUnitofWork _unitofWork;
        private readonly IService<CardDetail> _service;

        public CardDetailService(IUnitofWork unitofWork, IService<CardDetail> service)
        {
            _unitofWork = unitofWork;
            _repository = _unitofWork.GetRepository<CardDetail>();
            _service = service;
        }
        public async Task<PagedListViewModel<CardDetail>> GetAll(GetAllRequestViewModel model)
        {
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }
            return new PagedListViewModel<CardDetail>
            {
                TotalCount = result.Count(),
                ListResponse = await pagedResult.ToListAsync()
            };
        }
        public async Task<int> CreateUpdateCardDetails(CardDetail cardDetail)
        {
            var cardDetails = await _repository.GetAll().Where(x => x.DeviceUserId == cardDetail.DeviceUserId && !x.IsDeleted && x.CardNumber == cardDetail.CardNumber).FirstOrDefaultAsync();
            if (cardDetails != null)
            {
                cardDetails.CVV = cardDetail.CVV;
                cardDetails.CardNumber = cardDetail.CardNumber;
                cardDetails.ExpiryDate = cardDetail.ExpiryDate;
                cardDetails.NameOnCard = cardDetail.NameOnCard;
                _repository.Update(cardDetails);
            }
            else
            {
                await _repository.Add(cardDetail);
            }
            return _unitofWork.commit();
        }
    }
}
