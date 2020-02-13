using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace H2020.IPMDecisions.IDP.API.Providers
{
    public interface IAuthenticationProvider
    {
        Task<AuthenticationProviderResult<ApplicationUser>> FindUserAsync(Guid userId);
        Task<AuthenticationProviderResult<ApplicationClient>> ValidateApplicationClientAsync(HttpRequest request);
        Task<AuthenticationProviderResult<ApplicationUser>> ValidateUserAuthenticationAsync(UserForAuthenticationDto userDto);
    }
}