using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;

namespace H2020.IPMDecisions.IDP.API.Providers
{
    public interface IJWTProvider
    {
        string GenerateToken(List<Claim> claims, string audienceServerUrl);
        Task<List<Claim>> GetValidClaims(ApplicationUser user);
    }
}