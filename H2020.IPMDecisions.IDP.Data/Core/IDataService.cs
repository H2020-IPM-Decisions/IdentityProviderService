using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Data.Core.Repositories;

namespace H2020.IPMDecisions.IDP.Data.Core
{
    public interface IDataService : IDisposable
    {
        IApplicationClientRepository ApplicationClients { get; }
        IApplicationUserRepository ApplicationUsers { get; }
        Task CompleteAsync();
    }
}