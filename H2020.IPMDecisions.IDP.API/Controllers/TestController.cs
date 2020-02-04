using System;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("/api/test")]
    public class TestController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;
        private readonly IDataService dataService;
        private readonly IConfiguration config;

        public TestController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper,
            IDataService dataService,
            IConfiguration config)
        {
            this.dataService = dataService
                ?? throw new System.ArgumentNullException(nameof(dataService));
            this.config = config 
                ?? throw new ArgumentNullException(nameof(config));
            this.userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            this.signInManager = signInManager
                ?? throw new ArgumentNullException(nameof(signInManager));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }

        [AllowAnonymous]
        [HttpPost("Authenticate", Name = "AuthenticateUser2")]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthentificationDto userDto)
        {
            var isValidClient = await ValidateApplicationClientAsync();
            if (!isValidClient.Item1) return BadRequest(new { message = isValidClient.Item2 });

            var isAuthorize = await ValidateUserAuthenticationAsync(userDto);
            if (!isAuthorize.Item1) return BadRequest(new { message = isAuthorize.Item2 });

            var token = GenerateToken(isValidClient.Item3);
            return Ok(token);
        }

        private string GenerateToken(ApplicationClient client)
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

        private async Task<Tuple<bool, string, ApplicationClient>> ValidateApplicationClientAsync()
        {
            ApplicationClient client = null;
            var clientId = Request.Headers["client_id"];
            if (string.IsNullOrEmpty(clientId)) 
                return Tuple.Create(false, "Client Id is not set", client);

            client = await this.dataService.ApplicationClients.FindByIdAsync(Guid.Parse(clientId));
            if (client == null) 
                return Tuple.Create(false, "Invalid client Id", client);

            if (!client.Enabled) 
                return Tuple.Create(false, "Client is inactive", client);

            if (client.ApplicationClientType == ApplicationClientType.Confidential)
            {
                var clientSecret = Request.Headers["client_secret"];
                if (string.IsNullOrEmpty(clientSecret)) 
                    return Tuple.Create(false, "Client secret is not set", client);

                if (client.Base64Secret != clientSecret) 
                    return Tuple.Create(false, "Client secret do not match", client);
            }

            return Tuple.Create(true, "", client);
        }

        private async Task<Tuple<bool, string, ApplicationUser>> ValidateUserAuthenticationAsync(UserForAuthentificationDto userDto)
        {
            var user = await this.userManager.FindByNameAsync(userDto.Username);

            if (user == null) 
                return Tuple.Create(false, "Username or password is incorrect", user);

            // ToDo When Email confirmation available
            //if (!user.EmailConfirmed) 
                // return Tuple.Create(false, "Email not confirmed"", user);"

            var result = await this.signInManager.PasswordSignInAsync(user.UserName, userDto.Password, false, true);

            if (result.Succeeded)
                return Tuple.Create(true, "", user);            
            else if (result.IsLockedOut)
                return Tuple.Create(false, "User is lockout", user);
            else
                return Tuple.Create(false, "Username or password is incorrect", user);
        }

        [Authorize]
        [HttpGet("protected")]
        public string Protected()
        {
            return "Only if you have a valid token!";
        }
    }
}