using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.IdentityModel.Tokens;

namespace H2020.IPMDecisions.IDP.API.Providers
{
    public static class RefreshTokenProvider
    {
        public async static Task<string> GenerateRefreshToken(
            IDataService dataService,
            ApplicationUser user,
            ApplicationClient client)
        {

            var existingRefreshToken = await dataService
                .RefreshTokens
                .FindByCondition(r => r.ApplicationClientId == client.Id && r.UserId.ToString() == user.Id);

            if (existingRefreshToken != null)
            {
                dataService.RefreshTokens.Delete(existingRefreshToken);
            }

            RefreshToken refreshToken = new RefreshToken()
            {
                UserId = Guid.Parse(user.Id),
                ApplicationClientId = client.Id,
                ProtectedTicket = GenerateRefreshToken(),
            };

            dataService.RefreshTokens.Create(refreshToken);
            await dataService.CompleteAsync();

            return refreshToken.ProtectedTicket;
        }

        public static async Task<AuthenticationProviderResult<RefreshToken>> ValidateRefreshToken(
            IDataService dataService,
            ApplicationClient client,
            string refreshTokenTicket)
        {
            var response = new AuthenticationProviderResult<RefreshToken>()
            {
                IsSuccessful = false,
                ResponseMessage = "",
                Result = null
            };

            var existingRefreshToken = await dataService
               .RefreshTokens
               .FindByCondition(r => r.ApplicationClientId == client.Id
               && r.ProtectedTicket == refreshTokenTicket);

            if (existingRefreshToken == null)
            {
                response.ResponseMessage = "Invalid token";
                return response;
            }

            dataService.RefreshTokens.Delete(existingRefreshToken);

            RefreshToken newRefreshToken = new RefreshToken()
            {
                UserId = existingRefreshToken.UserId,
                ApplicationClientId = client.Id,
                ProtectedTicket = GenerateRefreshToken(),
            };

            dataService.RefreshTokens.Create(newRefreshToken);
            await dataService.CompleteAsync();

            response.IsSuccessful = true;
            response.Result = newRefreshToken;
            
            return response;
        }

        #region Helpers
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        #endregion
    }
}