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
            if (Request.Headers["grant_type"].ToString().ToLower() != "password") return BadRequest();
            
            var isValidClient = await AuthenticationProvider.ValidateApplicationClientAsync(this.Request, this.dataService);
            if (!isValidClient.IsSuccessful) return BadRequest(new { message = isValidClient.ResponseMessage });

            var isAuthorize = await AuthenticationProvider.ValidateUserAuthenticationAsync(this.dataService, this.signInManager, userDto);
            if (!isAuthorize.IsSuccessful) return BadRequest(new { message = isAuthorize.ResponseMessage });

            var claims = await JWTProvider.GetValidClaims(this.dataService, isAuthorize.Result);
            var token = JWTProvider.GenerateToken(this.config, claims, isValidClient.Result.Url);
            var refreshToken = await RefreshTokenProvider.GenerateRefreshToken(this.dataService, isAuthorize.Result, isValidClient.Result);

            return Ok(new { 
                token,
                token_type = "bearer",
                refreshToken});
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("authenticate/token", Name = "AuthenticateUserWithToken")]
        [RequiredClientHeader("client_id", "client_secret", "grant_type", "refresh_token")]
        // POST: api/Accounts/authenticate/token
        public async Task<IActionResult> AuthenticateToken()
        {
            if (Request.Headers["grant_type"].ToString().ToLower() != "refresh_token") return BadRequest();

            var isValidClient = await AuthenticationProvider.ValidateApplicationClientAsync(this.Request, this.dataService);
            if (!isValidClient.IsSuccessful) return BadRequest(new { message = isValidClient.ResponseMessage });

            var refreshTokenFromHeader = Request.Headers["refresh_token"].ToString();
            var isValidRefreshToken = await RefreshTokenProvider.ValidateRefreshToken(this.dataService, isValidClient.Result, refreshTokenFromHeader);
            if (!isValidRefreshToken.IsSuccessful) return BadRequest(new { message = isValidRefreshToken.ResponseMessage });

            var isAuthorize = await AuthenticationProvider.FindUserAsync(this.dataService, isValidRefreshToken.Result.UserId);
            if (!isAuthorize.IsSuccessful) return BadRequest(new { message = isAuthorize.ResponseMessage });

            var claims = await JWTProvider.GetValidClaims(this.dataService, isAuthorize.Result);
            var token = JWTProvider.GenerateToken(this.config, claims, isValidClient.Result.Url);
            
            return Ok(new
            {
                token,
                token_type = "bearer",
                refreshToken = isValidRefreshToken.Result.ProtectedTicket
            });
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