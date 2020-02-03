using System;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Data.Core;

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
            var isValidClient = await ValidateClient();

            if (!isValidClient.Item1) return BadRequest(isValidClient.Item2);

            IActionResult isAuthorize = await ValidateUserAuthenticationAsync(userDto);

            return isAuthorize;
        }

        private async Task<Tuple<bool, string>> ValidateClient()
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

        private async Task<IActionResult> ValidateUserAuthenticationAsync(UserForAuthentificationDto userDto)
        {
            var user = await this.userManager.FindByNameAsync(userDto.Username);

            if (user == null) return BadRequest(new { message = "Username or password is incorrect" });

            // ToDo When Email confirmation available
            //if (!user.EmailConfirmed) return BadRequest(new { message = "Email not confirmed" });

            var result = await this.signInManager.PasswordSignInAsync(user.UserName, userDto.Password, false, true);

            if (result.Succeeded)
            {
                var userToReturn = this.mapper.Map<UserDto>(user);
                // ToDo generate JWT here and return
                return Ok(userToReturn);
            }
            else if (result.IsLockedOut)
            {
                return BadRequest(new { message = "Username lockout" });
            }
            else
            {
                return BadRequest(new { message = "Username or password is incorrect" });
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