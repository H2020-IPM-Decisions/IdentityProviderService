using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace H2020.IPMDecisions.IDP.BLL.Providers
{
    public class JWTProvider : IJWTProvider
    {
        private readonly IConfiguration config;
        private readonly IDataService dataService;

        public JWTProvider(
            IDataService dataService,
            IConfiguration config)
        {
            this.config = config
                ?? throw new ArgumentNullException(nameof(config));
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
        }

        public string GenerateToken(
                List<Claim> claims,
                string audienceServerUrl)
        {
            var tokenLifetimeMinutes = this.config["JwtSettings:TokenLifetimeMinutes"];
            var issuerServerUrl = this.config["JwtSettings:IssuerServerUrl"];
            var jwtSecretKey = this.config["JwtSettings:SecretKey"];

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: issuerServerUrl,
                audience: audienceServerUrl,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(double.Parse(tokenLifetimeMinutes)),
                signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }
        
        public async Task<List<Claim>> GetValidClaims(ApplicationUser user, IList<string> userRoles, IList<Claim> userClaims)
        {
            IdentityOptions _options = new IdentityOptions();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, await JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64),
                new Claim(_options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
                new Claim(_options.ClaimsIdentity.UserNameClaimType, user.UserName)
            };

            claims.AddRange(userClaims);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await this.dataService.RoleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await this.dataService.RoleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }

        #region Helpers
        private static Func<Task<string>> JtiGenerator =>
                 () => Task.FromResult(Guid.NewGuid().ToString());

        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);
        #endregion
    }
}