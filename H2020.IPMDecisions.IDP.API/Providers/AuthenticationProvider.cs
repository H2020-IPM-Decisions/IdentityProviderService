using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace H2020.IPMDecisions.IDP.API.Providers
{
    public static class AuthenticationProvider
    {
        public static async Task<List<Claim>> GetValidClaims(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationUser user)
        {
            IdentityOptions _options = new IdentityOptions();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, await JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64),
                new Claim(_options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
                new Claim(_options.ClaimsIdentity.UserNameClaimType, user.UserName)
            };

            var userClaims = await userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }

        public static string GenerateToken(IConfiguration config,
                List<Claim> claims)
        {
            var tokenLifetimeMinutes = config["JwtSettings:TokenLifetimeMinutes"];
            var authorizationServerUrl = config["JwtSettings:AuthorizationServerUrl"];
            var audienceServerUrl = config["JwtSettings:ApiGatewayServerUrl"];
            var authorizationSecretKey = config["JwtSettings:AuthorizationServerSecret"];

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authorizationSecretKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                issuer: authorizationServerUrl,
                audience: audienceServerUrl,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(double.Parse(tokenLifetimeMinutes)),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return tokenString;
        }

        public static async Task<AuthenticationProviderResult<ApplicationClient>> ValidateApplicationClientAsync(
            HttpRequest request,
            IDataService dataService)
        {
            var response = new AuthenticationProviderResult<ApplicationClient>()
            {
                IsSuccessful = false,
                ResponseMessage = "",
                Result = null
            };
            var clientId = request.Headers["client_id"];
            if (string.IsNullOrEmpty(clientId))
            {
                response.ResponseMessage = "Client Id is not set";
                return response;
            }

            var client = await dataService.ApplicationClients.FindByIdAsync(Guid.Parse(clientId));
            if (client == null)
            {
                response.ResponseMessage = "Invalid client Id";
                return response;
            }

            if (!client.Enabled)
            {
                response.ResponseMessage = "Client is inactive";
                return response;
            }

            if (client.ApplicationClientType == ApplicationClientType.Confidential)
            {
                var clientSecret = request.Headers["client_secret"];
                if (string.IsNullOrEmpty(clientSecret))
                {
                    response.ResponseMessage = "Client secret is not set";
                    return response;
                };

                if (client.Base64Secret != clientSecret)
                {
                    response.ResponseMessage = "Client secret do not match";
                    return response;
                }
            }

            response.IsSuccessful = true;
            response.Result = client;
            return response;
        }

        public static async Task<AuthenticationProviderResult<ApplicationUser>> ValidateUserAuthenticationAsync(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            UserForAuthentificationDto userDto)
        {
            var response = new AuthenticationProviderResult<ApplicationUser>()
            {
                IsSuccessful = false,
                ResponseMessage = "",
                Result = null
            };

            var user = await userManager.FindByNameAsync(userDto.Username);

            if (user == null)
            {
                response.ResponseMessage = "Username or password is incorrect";
                return response;
            }

            // ToDo When Email confirmation available
            //if (!user.EmailConfirmed) 
            // return Tuple.Create(false, "Email not confirmed"", user);"

            var result = await signInManager.PasswordSignInAsync(user.UserName, userDto.Password, false, true);

            if (result.Succeeded)
            {
                response.IsSuccessful = true;
                response.Result = user;
                return response;
            }

            else if (result.IsLockedOut)
            {
                response.ResponseMessage = "User is lockout";
                return response;
            }
            else
            {
                response.ResponseMessage = "Username or password is incorrect";
                return response;
            }
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