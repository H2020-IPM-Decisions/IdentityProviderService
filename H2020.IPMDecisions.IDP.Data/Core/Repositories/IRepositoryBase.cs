using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;

namespace H2020.IPMDecisions.IDP.Data.Core.Repositories
{
    public interface IRepositoryBase<T>
    {
        void Create(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> FindAllAsync();
        Task<IEnumerable<T>> FindAllAsync(BaseResourceParameter resourceParameter);
        Task<T> FindByIdAsync(Guid id);
        void Update(T entity);
    }
}