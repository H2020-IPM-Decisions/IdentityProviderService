using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace H2020.IPMDecisions.IDP.BLL.Providers
{
    public interface IAuthenticationProvider
    {
        Task<IList<Claim>> GetUserClaimsAsync(ApplicationUser user);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<AuthenticationProviderResult<ApplicationUser>> FindUserAsync(Guid userId);
        Task<AuthenticationProviderResult<ApplicationClient>> ValidateApplicationClientAsync(HttpRequest request);
        Task<AuthenticationProviderResult<ApplicationUser>> ValidateUserAuthenticationAsync(UserForAuthenticationDto userDto);
    }
}