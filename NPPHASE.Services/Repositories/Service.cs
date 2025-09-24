using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Interface;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class Service<TEntity> : IService<TEntity> where TEntity : class, IAuditabeEntity
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<TEntity> repository;
        public Service(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
            repository = _unitofWork.GetRepository<TEntity>();
        }

        public virtual async Task<bool> AddRangeAsync(List<TEntity> entities)
        {
            await repository.AddRangeAsync(entities);
            _ = _unitofWork.commit();
            return true;

        }

        public virtual async Task<TEntity> Create(TEntity entity)
        {
            _ = await repository.Add(entity);
            _ = _unitofWork.commit();
            return entity;
        }

        public IQueryable<TEntity> GetAllAsync(GetAllRequestViewModel model)
        {
            var result = repository.GetAll().Where(t => !t.IsDeleted);
            
            if (model.FromDate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.CreationDate.Date >= model.FromDate.Value.Date && t.CreationDate.Date <= model.ToDate.Value.Date);
            }
            return result;
        }

        public IQueryable<TEntity> GetAllDataAsync(GetAllRequestViewModel model)
        {
            var result = repository.GetAll().Where(t => !t.IsDeleted);

            if (model.FromDate.HasValue && model.ToDate.HasValue)
            {
                result = result.Where(t => t.CreationDate >= model.FromDate.Value && t.CreationDate <= model.ToDate.Value);
            }
            return result;
        }

        public async Task<PagedListViewModel<TEntity>> GetAll(GetAllRequestViewModel model)
        {
            var result = this.GetAllAsync(model);
            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }
            return new PagedListViewModel<TEntity>
            {
                TotalCount = result.Count(),
                ListResponse = await pagedResult.ToListAsync()
            };
        }

        public virtual async Task<TEntity> GetByIdAsync<Tkey>(Tkey id)
        {
            var result = await repository.GetByIdAsync(id);
            if (result != null && result.IsDeleted == false)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public virtual bool HardDelete<Tkey>(Tkey id)
        {
            var data = repository.Get(id);
            _ = repository.Delete(data);
            _ = _unitofWork.commit();
            return true;
        }

        public virtual bool SoftDelete<Tkey>(Tkey id)
        {
            var data = repository.Get(id);
            if (data is IAuditabeEntity)
            {
                data.GetType().GetProperty("IsDeleted").SetValue(data, true);
            }
            _ = repository.Update(data);
            _ = _unitofWork.commit();
            return true;
        }

        public virtual TEntity Update(TEntity entity)
        {
            _ = repository.Update(entity);
            _ = _unitofWork.commit();
            return entity;
        }

        public virtual bool UpdateRange(List<TEntity> entities)
        {
            repository.UpdateRange(entities);
            _ = _unitofWork.commit();
            return true;
        }

        public virtual async Task<bool> DeleteRange(List<int> ids)
        {
            await repository.DeleteRange(ids);
            _ = _unitofWork.commit();
            return true;
        }

        
    }
}
