using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;

namespace NPPHASE.Services.IRepositories
{
    public interface IService<TEntity> where TEntity : class
    {
        Task<TEntity> Create(TEntity entity);
        TEntity Update(TEntity entity);
        IQueryable<TEntity> GetAllAsync(GetAllRequestViewModel model);
        IQueryable<TEntity> GetAllDataAsync(GetAllRequestViewModel model);
        Task<PagedListViewModel<TEntity>> GetAll(GetAllRequestViewModel model);
        Task<TEntity> GetByIdAsync<Tkey>(Tkey id);
        bool SoftDelete<Tkey>(Tkey id);
        Task<bool> DeleteRange(List<int> id);
        bool HardDelete<Tkey>(Tkey id);
        bool UpdateRange(List<TEntity> entities);
        Task<bool> AddRangeAsync(List<TEntity> entities);        
    }
}
