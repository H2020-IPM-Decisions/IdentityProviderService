using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Models;

namespace H2020.IPMDecisions.IDP.BLL.Providers
{
    public interface IRefreshTokenProvider
    {
        Task<string> GenerateRefreshToken(ApplicationUser user, ApplicationClient client);
        Task<AuthenticationProviderResult<RefreshToken>> ValidateRefreshToken(ApplicationClient client, string refreshTokenTicket);
    }
}