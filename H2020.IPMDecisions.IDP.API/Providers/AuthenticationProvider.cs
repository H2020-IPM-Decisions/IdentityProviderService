using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        public static string GenerateToken(IConfiguration config,
            ApplicationClient client)
        {
            var tokenLifetimeMinutes = config["JwtSettings:TokenLifetimeMinutes"];
            var authorizationServerUrl = config["JwtSettings:AuthorizationServerUrl"];
            var audienceServerUrl = config["JwtSettings:ApiGatewayServerUrl"];

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(client.Base64Secret));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                issuer: authorizationServerUrl,
                audience: audienceServerUrl,
                claims: new List<Claim>(),
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(double.Parse(tokenLifetimeMinutes)),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return tokenString;
        }

        public static async Task<Tuple<bool, string, ApplicationClient>> ValidateApplicationClientAsync(
            HttpRequest request, 
            IDataService dataService)
        {
            ApplicationClient client = null;
            var clientId = request.Headers["client_id"];
            if (string.IsNullOrEmpty(clientId))
                return Tuple.Create(false, "Client Id is not set", client);

            client = await dataService.ApplicationClients.FindByIdAsync(Guid.Parse(clientId));
            if (client == null)
                return Tuple.Create(false, "Invalid client Id", client);

            if (!client.Enabled)
                return Tuple.Create(false, "Client is inactive", client);

            if (client.ApplicationClientType == ApplicationClientType.Confidential)
            {
                var clientSecret = request.Headers["client_secret"];
                if (string.IsNullOrEmpty(clientSecret))
                    return Tuple.Create(false, "Client secret is not set", client);

                if (client.Base64Secret != clientSecret)
                    return Tuple.Create(false, "Client secret do not match", client);
            }

            return Tuple.Create(true, "", client);
        }

        public static async Task<Tuple<bool, string, ApplicationUser>> ValidateUserAuthenticationAsync(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            UserForAuthentificationDto userDto)
        {
            var user = await userManager.FindByNameAsync(userDto.Username);

            if (user == null)
                return Tuple.Create(false, "Username or password is incorrect", user);

            // ToDo When Email confirmation available
            //if (!user.EmailConfirmed) 
            // return Tuple.Create(false, "Email not confirmed"", user);"

            var result = await signInManager.PasswordSignInAsync(user.UserName, userDto.Password, false, true);

            if (result.Succeeded)
                return Tuple.Create(true, "", user);
            else if (result.IsLockedOut)
                return Tuple.Create(false, "User is lockout", user);
            else
                return Tuple.Create(false, "Username or password is incorrect", user);
        }
    }
}