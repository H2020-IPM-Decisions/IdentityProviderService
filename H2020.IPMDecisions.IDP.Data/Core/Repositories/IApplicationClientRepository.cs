using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;

namespace H2020.IPMDecisions.IDP.Data.Core.Repositories
{
    public interface IApplicationClientRepository : IRepositoryBase<ApplicationClient, ApplicationClientResourceParameter>
    {
        Task<ApplicationClient> FindByNameAsync(string name);
    }
}