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
using static H2020.IPMDecisions.IDP.API.Providers.Constants.Strings;

namespace H2020.IPMDecisions.IDP.API.Providers
{
    public static class AuthenticationProvider
    {
        public static string GenerateToken(IConfiguration config,
            ApplicationClient client,
            ApplicationUser user)
        {
            var tokenLifetimeMinutes = config["JwtSettings:TokenLifetimeMinutes"];
            var authorizationServerUrl = config["JwtSettings:AuthorizationServerUrl"];
            var audienceServerUrl = config["JwtSettings:ApiGatewayServerUrl"];

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(client.Base64Secret));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                // new Claim(JwtRegisteredClaimNames.Sub, ""),
                // new Claim(ClaimTypes.Name, ""),
                // new Claim(ClaimTypes.Role, "SuperAdmin")
            };

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
    }
    
}