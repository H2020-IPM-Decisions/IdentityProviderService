using System;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.API.Filters;
using H2020.IPMDecisions.IDP.API.Providers;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;
        private readonly IDataService dataService;
        private readonly IConfiguration config;

        public AccountsController(
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper,
            IDataService dataService,
            IConfiguration config)
        {
            this.signInManager = signInManager
                ?? throw new ArgumentNullException(nameof(signInManager));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            this.dataService = dataService 
                ?? throw new ArgumentNullException(nameof(dataService));
            this.config = config 
                ?? throw new ArgumentNullException(nameof(config));
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("register", Name = "RegisterUser")]
        // POST: api/Accounts/register
        public async Task<ActionResult<UserDto>> Register([FromBody] UserForRegistrationDto userForRegistration)
        {
            var userEntity = this.mapper.Map<ApplicationUser>(userForRegistration);

            var result = await this.dataService.UserManager.CreateAsync(userEntity, userForRegistration.Password);

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

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("authenticate", Name = "AuthenticateUser")]
        [RequiredClientHeader("client_id", "client_secret", "grant_type")]
        // POST: api/Accounts/authenticate
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto userDto)
        {
            var isValidClient = await AuthenticationProvider.ValidateApplicationClientAsync(this.Request, this.dataService);
            if (!isValidClient.IsSuccessful) return BadRequest(new { message = isValidClient.ResponseMessage });

            var isAuthorize = await AuthenticationProvider.ValidateUserAuthenticationAsync(this.dataService, this.signInManager, userDto);
            if (!isAuthorize.IsSuccessful) return BadRequest(new { message = isAuthorize.ResponseMessage });

            var claims = await AuthenticationProvider.GetValidClaims(this.dataService, isAuthorize.Result);
            var token = AuthenticationProvider.GenerateToken(this.config, claims, isValidClient.Result.Url);
            
            return Ok(new { Token = token });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST");
            return Ok();
        }
    }
}