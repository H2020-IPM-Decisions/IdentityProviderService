using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.IDP.Data.Core.Repositories
{
    public interface IRepositoryBase<T>
    {
        void Create(T entity);
        void Delete(T entity);
        Task<List<T>> FindAllAsync();
        Task<T> FindByIdAsync(Guid id);
        void Update(T entity);
    }
}