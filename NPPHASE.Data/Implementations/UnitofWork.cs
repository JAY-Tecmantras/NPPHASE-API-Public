
using NPPHASE.Data.Context;
using NPPHASE.Data.Interface;

namespace NPPHASE.Data.Implementations
{
    public class UnitofWork : IUnitofWork
    {
        private readonly NPPHASEApiDbContext dbcontext;
        private Dictionary<Type, object> repositories;

        public UnitofWork(NPPHASEApiDbContext context)
        {
            dbcontext = context;
        }
        public int commit()
        {
            dbcontext.SaveChanges();
            return 1;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            repositories ??= new Dictionary<Type, object>();
            var type = typeof(TEntity);
            if (!repositories.ContainsKey(type))
            {
                repositories[type] = new Repository<TEntity>(dbcontext);
            }
            return (IRepository<TEntity>)repositories[type];
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(obj: this);
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbcontext != null)
                {
                  //  dbcontext.Dispose();
                    //dbontext = null;
                }
            }
        }
    }
}

