﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.Interface
{
    public interface IRepository<T>
    {
        T Get<Tkey>(Tkey id);
        IQueryable<T> GetAll();
        Task<T> GetByIdAsync<Tkey>(Tkey id);
        Task<EntityState> Add(T entity);
        EntityState Update(T entity);
        EntityState Delete(T entity);
        void UpdateRange(List<T> entities);
        Task AddRangeAsync(List<T> entities);
        Task<bool> DeleteRange(List<int> idsToDelete);
        void RemoveRange(List<T> entities);
    }
}
