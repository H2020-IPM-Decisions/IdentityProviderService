using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using Microsoft.AspNetCore.Identity;

namespace H2020.IPMDecisions.IDP.Data.Core.Repositories
{
    public interface IApplicationUserRepository : IRepositoryBase<ApplicationUser, ApplicationUserResourceParameter>
    {
        Task<IdentityResult> CreateAsync(ApplicationUser entity, string password);
        Task<IdentityResult> DeleteAsync(ApplicationUser entity);
        Task<ApplicationUser> FindByNameAsync(string name);
        Task<IList<Claim>> GetClaimsAsync(ApplicationUser entity);
        Task<IList<string>> GetRolesAsync(ApplicationUser entity);
    }
}