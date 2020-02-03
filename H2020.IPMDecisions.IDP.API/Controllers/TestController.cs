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

        public TestController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper,
            IDataService dataService)
        {
            this.dataService = dataService
                ?? throw new System.ArgumentNullException(nameof(dataService));
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

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                issuer: "https://localhost:5001",
                audience: "https://localhost:5001",
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return Ok(new { Token = tokenString });
        }

        private async Task<Tuple<bool, string>> ValidateApplicationClientAsync()
        {
            var clientId = Request.Headers["client_id"];
            if (string.IsNullOrEmpty(clientId)) return Tuple.Create(false,"Client Id is not set") ;

            var client = await this.dataService.ApplicationClients.FindByIdAsync(Guid.Parse(clientId));
            if (client == null) return Tuple.Create(false, "Invalid client Id");

            if (!client.Enabled) return Tuple.Create(false, "Client is inactive");

            if (client.ApplicationClientType == ApplicationClientType.Confidential)
            {
                var clientSecret = Request.Headers["client_secret"];
                if (string.IsNullOrEmpty(clientSecret)) return Tuple.Create(false, "Client secret is not set");

                if (client.Base64Secret != clientSecret) return Tuple.Create(false, "Client secret do not match");
            }

            return Tuple.Create(true, "");
        }

        private async Task<Tuple<bool, string, ApplicationUser>> ValidateUserAuthenticationAsync(UserForAuthentificationDto userDto)
        {
            var user = await this.userManager.FindByNameAsync(userDto.Username);

            if (user == null) return Tuple.Create(false, "Username or password is incorrect", user);

            // ToDo When Email confirmation available
            //if (!user.EmailConfirmed) return Tuple.Create(false, "Email not confirmed"", user);"

            var result = await this.signInManager.PasswordSignInAsync(user.UserName, userDto.Password, false, true);

            if (result.Succeeded)
            {
                return Tuple.Create(true, "", user);
            }
            else if (result.IsLockedOut)
            {
                return Tuple.Create(false, "User is lockout", user);
            }
            else
            {
                return Tuple.Create(false, "Username or password is incorrect", user);
            }
        }

        [Authorize]
        [HttpGet("protected")]
        public string Protected()
        {
            return "Only if you have a valid token!";
        }
    }
}