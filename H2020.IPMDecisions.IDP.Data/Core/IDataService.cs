using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Core.Repositories;
using Microsoft.AspNetCore.Identity;

namespace H2020.IPMDecisions.IDP.Data.Core
{
    public interface IDataService : IDisposable
    {
        IApplicationClientRepository ApplicationClients { get; }
        Task CompleteAsync();
        UserManager<ApplicationUser> UserManager { get; }
        IUserManagerExtensionRepository UserManagerExtensions { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        RoleManager<IdentityRole> RoleManager { get; }
    }
}