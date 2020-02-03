using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.IDP.Data.Core.Repositories
{
    public interface IRepositoryBase<T>
    {
        Task<List<T>> GetAll();
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}