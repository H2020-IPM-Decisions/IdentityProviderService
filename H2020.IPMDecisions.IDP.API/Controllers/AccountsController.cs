using System;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.API.Providers;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;
        private readonly IDataService dataService;
        private readonly IConfiguration config;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper,
            IDataService dataService,
            IConfiguration config, 
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            this.signInManager = signInManager
                ?? throw new ArgumentNullException(nameof(signInManager));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            this.dataService = dataService 
                ?? throw new ArgumentNullException(nameof(dataService));
            this.config = config 
                ?? throw new ArgumentNullException(nameof(config));
            this.roleManager = roleManager 
                ?? throw new ArgumentNullException(nameof(roleManager));
        }

        [AllowAnonymous]
        [HttpPost("Register", Name = "RegisterUser")]
        // POST: api/Accounts/Register
        public async Task<ActionResult<UserDto>> Register([FromBody] UserForRegistrationDto userForRegistration)
        {
            var userEntity = this.mapper.Map<ApplicationUser>(userForRegistration);

            var result = await this.userManager.CreateAsync(userEntity, userForRegistration.Password);

            if (result.Succeeded)
            {
                //ToDo Generate Email token and return

                var userToReturn = this.mapper.Map<UserDto>(userEntity);

                return CreatedAtRoute("GetUser",
                new { userId = userToReturn.Id },
                userToReturn);
            }

            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("Authenticate", Name = "AuthenticateUser")]
        // POST: api/Accounts/Authenticate
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto userDto)
        {
            var isValidClient = await AuthenticationProvider.ValidateApplicationClientAsync(this.Request, this.dataService);
            if (!isValidClient.IsSuccessful) return BadRequest(new { message = isValidClient.ResponseMessage });

            var isAuthorize = await AuthenticationProvider.ValidateUserAuthenticationAsync(this.userManager, this.signInManager, userDto);
            if (!isAuthorize.IsSuccessful) return BadRequest(new { message = isAuthorize.ResponseMessage });

            var claims = await AuthenticationProvider.GetValidClaims(this.userManager, this.roleManager, isAuthorize.Result);
            var token = AuthenticationProvider.GenerateToken(this.config, claims, isValidClient.Result.Url);
            
            return Ok(new { Token = token });
        }

        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST");
            return Ok();
        }
    }
}