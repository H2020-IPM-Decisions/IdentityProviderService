using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.BLL.Helpers;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Models;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace H2020.IPMDecisions.IDP.BLL.Providers
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        private readonly IDataService dataService;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration config;
        private readonly IJsonStringLocalizer jsonStringLocalizer;

        public AuthenticationProvider(
            IDataService dataService,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config, 
            IJsonStringLocalizer jsonStringLocalizer)
        {
            this.signInManager = signInManager
                ?? throw new ArgumentNullException(nameof(signInManager));
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
            this.config = config
                ?? throw new ArgumentNullException(nameof(config));
            this.jsonStringLocalizer = jsonStringLocalizer 
                ?? throw new ArgumentNullException(nameof(jsonStringLocalizer));
        }

        public async Task<AuthenticationProviderResult<ApplicationClient>> ValidateApplicationClientAsync(HttpRequest request)
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

            if (!Guid.TryParse(clientId, out Guid validClientID))
            {
                response.ResponseMessage = "Invalid client Id";
                return response;
            }

            var client = await this.dataService.ApplicationClients.FindByIdAsync(validClientID);
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

        public async Task<AuthenticationProviderResult<ApplicationUser>> ValidateUserAuthenticationAsync(
            UserForAuthenticationDto userDto)
        {
            var response = new AuthenticationProviderResult<ApplicationUser>()
            {
                IsSuccessful = false,
                ResponseMessage = "",
                Result = null
            };

            var user = await this.dataService.UserManager.FindByEmailAsync(userDto.Email);

            if (user == null)
            {
                response.ResponseMessage = this.jsonStringLocalizer["authentification.username_password_error"].ToString();
                return response;
            }

            var registrationTime = double.Parse(config["EmailConfirmationAllowanceHours"]);
            if (!user.EmailConfirmed && DateTime.Now > user.RegistrationDate.AddHours(registrationTime))
            {
                response.ResponseMessage = this.jsonStringLocalizer["authentification.email_not_confirmed"].ToString();
                return response;
            }

            var result = await this.signInManager.PasswordSignInAsync(user.UserName, userDto.Password, false, true);

            if (result.Succeeded)
            {
                await UpdateLastValidAccessAsync(user);
                response.IsSuccessful = true;
                response.Result = user;
                return response;
            }
            else if (result.IsLockedOut)
            {
                response.ResponseMessage = this.jsonStringLocalizer["authentification.email_not_confirmed"].ToString();
                return response;
            }
            else
            {
                response.ResponseMessage = this.jsonStringLocalizer["authentification.user_lockout"].ToString();
                return response;
            }
        }

        public async Task<AuthenticationProviderResult<ApplicationUser>> ValidateUserAsync(Guid userId)
        {
            var response = new AuthenticationProviderResult<ApplicationUser>()
            {
                IsSuccessful = false,
                ResponseMessage = "",
                Result = null
            };

            var user = await this.dataService.UserManager.FindByIdAsync(userId.ToString());
            response.ResponseMessage = this.jsonStringLocalizer["authentification.unauthorized_user"].ToString();
            if (user == null)
                return response;

            if (!await this.signInManager.CanSignInAsync(user))
                return response;

            if (this.dataService.UserManager.SupportsUserLockout && await this.dataService.UserManager.IsLockedOutAsync(user))
            {
                response.ResponseMessage = this.jsonStringLocalizer["authentification.user_lockout"].ToString();
                return response;
            }

            var registrationTime = double.Parse(config["EmailConfirmationAllowanceHours"]);
            if (!user.EmailConfirmed && DateTime.Now > user.RegistrationDate.AddHours(registrationTime))
            {
                response.ResponseMessage = this.jsonStringLocalizer["authentification.email_not_confirmed"].ToString();
                return response;
            }

            await UpdateLastValidAccessAsync(user);
            response.IsSuccessful = true;
            response.ResponseMessage = "";
            response.Result = user;
            return response;
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await this.dataService.UserManager.GetRolesAsync(user);
        }

        public async Task<IList<Claim>> GetUserClaimsAsync(ApplicationUser user)
        {
            return await this.dataService.UserManager.GetClaimsAsync(user);
        }

        public async Task UpdateLastValidAccessAsync(ApplicationUser user)
        {
            // Reset inactive parameters
            user.LastValidAccess = DateTime.Now;
            user.InactiveEmailsSent = 0;
            await this.dataService.UserManager.UpdateAsync(user);
        }
    }
}