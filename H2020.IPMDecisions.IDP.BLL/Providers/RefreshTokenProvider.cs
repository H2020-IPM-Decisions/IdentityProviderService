using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Models;
using H2020.IPMDecisions.IDP.Data.Core;

namespace H2020.IPMDecisions.IDP.BLL.Providers
{
    public class RefreshTokenProvider : IRefreshTokenProvider
    {
        private readonly IDataService dataService;

        public RefreshTokenProvider(
            IDataService dataService)
        {
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
        }

        public async Task<string> GenerateRefreshToken(
            ApplicationUser user,
            ApplicationClient client)
        {
            var existingRefreshToken = await this.dataService
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
                ExpiresUtc = DateTime.Now.AddMinutes(client.RefreshTokenLifeTime).ToUniversalTime()
            };

            dataService.RefreshTokens.Create(refreshToken);
            await dataService.CompleteAsync();

            return refreshToken.ProtectedTicket;
        }

        public async Task<AuthenticationProviderResult<RefreshToken>> ValidateRefreshToken(
            ApplicationClient client,
            string refreshTokenTicket)
        {
            var response = new AuthenticationProviderResult<RefreshToken>()
            {
                IsSuccessful = false,
                ResponseMessage = "",
                Result = null
            };

            var existingRefreshToken = await this.dataService
               .RefreshTokens
               .FindByCondition(r => r.ApplicationClientId == client.Id
               && r.ProtectedTicket == refreshTokenTicket);

            if (existingRefreshToken == null)
            {
                response.ResponseMessage = "Invalid token";
                return response;
            }

            if (existingRefreshToken.ExpiresUtc < DateTime.Now.ToUniversalTime())
            {
                response.ResponseMessage = "Token expired";
                return response;
            }
            
            response.IsSuccessful = true;
            response.Result = existingRefreshToken;
            return response;
        }

        #region Helpers
        private static string GenerateRefreshToken()
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