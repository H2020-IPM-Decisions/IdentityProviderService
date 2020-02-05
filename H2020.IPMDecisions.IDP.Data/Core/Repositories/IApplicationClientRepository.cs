using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;

namespace H2020.IPMDecisions.IDP.Data.Core.Repositories
{
    public interface IApplicationClientRepository : IRepositoryBase<ApplicationClient>
    {
        Task<ApplicationClient> FindByNameAsync(string name);
    }
}