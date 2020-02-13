using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace H2020.IPMDecisions.IDP.API.Providers
{
    public static class AuthenticationProvider
    {
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
            IDataService dataService,
            SignInManager<ApplicationUser> signInManager,
            UserForAuthenticationDto userDto)
        {
            var response = new AuthenticationProviderResult<ApplicationUser>()
            {
                IsSuccessful = false,
                ResponseMessage = "",
                Result = null
            };

            var user = await dataService.UserManager.FindByNameAsync(userDto.Username);

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

        public static async Task<AuthenticationProviderResult<ApplicationUser>> FindUserAsync(
            IDataService dataService,
            Guid userId)
        {
            var response = new AuthenticationProviderResult<ApplicationUser>()
            {
                IsSuccessful = false,
                ResponseMessage = "",
                Result = null
            };

            var user = await dataService.UserManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                response.ResponseMessage = "User is incorrect";
                return response;
            }
            response.IsSuccessful = true;
            response.Result = user;
            return response;

        }
    }
}