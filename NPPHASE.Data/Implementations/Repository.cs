using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Context;
using NPPHASE.Data.Interface;

namespace NPPHASE.Data.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly NPPHASEApiDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(NPPHASEApiDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<EntityState> Add(T entity)
        {
            var entityState = await _dbSet.AddAsync(entity);
            return entityState.State;
        }

        public async Task AddRangeAsync(List<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<bool> DeleteRange(List<int> idsToDelete)
        {
            if (idsToDelete == null || idsToDelete.Count == 0)
            {
                return false;
            }

            var entityType = _context.Model.FindEntityType(typeof(T));
            var primaryKeyProperty = entityType.FindPrimaryKey().Properties.FirstOrDefault();

            if (primaryKeyProperty == null)
            {
                throw new InvalidOperationException("Entity does not have a primary key.");
            }

            var isAuditabeEntity = typeof(IAuditabeEntity).IsAssignableFrom(typeof(T));

            foreach (var id in idsToDelete)
            {
                var entity = Activator.CreateInstance<T>();
                primaryKeyProperty.PropertyInfo.SetValue(entity, id);

                if (isAuditabeEntity && entity is IAuditabeEntity auditabeEntity)
                {
                    auditabeEntity.IsDeleted = true;
                }

                _context.Attach(entity);
                _context.Entry(entity).Property("IsDeleted").IsModified = true;
            }
            return true;
        }
        public EntityState Delete(T entity)
        {
            var entityState = _dbSet.Remove(entity);
            return entityState.State;
        }

        public T Get<Tkey>(Tkey id)
        {
            return _dbSet.Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T> GetByIdAsync<Tkey>(Tkey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public EntityState Update(T entity)
        {
            return _dbSet.Update(entity).State;
        }

        public void UpdateRange(List<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public void RemoveRange(List<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
