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
        private readonly IMapper mapper;
        private readonly IDataService dataService;
        private readonly IAuthenticationProvider authenticationProvider;
        private readonly IJWTProvider jWTProvider;
        private readonly IRefreshTokenProvider refreshTokenProvider;

        public AccountsController(
            IMapper mapper,
            IDataService dataService,
            IAuthenticationProvider authenticationProvider,
            IJWTProvider jWTProvider,
            IRefreshTokenProvider refreshTokenProvider)
        {
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
            this.authenticationProvider = authenticationProvider 
                ?? throw new ArgumentNullException(nameof(authenticationProvider));
            this.jWTProvider = jWTProvider 
                ?? throw new ArgumentNullException(nameof(jWTProvider));
            this.refreshTokenProvider = refreshTokenProvider 
                ?? throw new ArgumentNullException(nameof(refreshTokenProvider));
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
            
            var isValidClient = await this.authenticationProvider.ValidateApplicationClientAsync(this.Request);
            if (!isValidClient.IsSuccessful) return BadRequest(new { message = isValidClient.ResponseMessage });

            var isAuthorize = await this.authenticationProvider.ValidateUserAuthenticationAsync(userDto);
            if (!isAuthorize.IsSuccessful) return BadRequest(new { message = isAuthorize.ResponseMessage });

            var claims = await this.jWTProvider.GetValidClaims(isAuthorize.Result);
            var token = this.jWTProvider.GenerateToken(claims, isValidClient.Result.Url);
            var refreshToken = await this.refreshTokenProvider.GenerateRefreshToken(isAuthorize.Result, isValidClient.Result);

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

            var isValidClient = await this.authenticationProvider.ValidateApplicationClientAsync(this.Request);
            if (!isValidClient.IsSuccessful) return BadRequest(new { message = isValidClient.ResponseMessage });

            var refreshTokenFromHeader = Request.Headers["refresh_token"].ToString();
            var isValidRefreshToken = await this.refreshTokenProvider.ValidateRefreshToken(isValidClient.Result, refreshTokenFromHeader);
            if (!isValidRefreshToken.IsSuccessful) return BadRequest(new { message = isValidRefreshToken.ResponseMessage });

            var isAuthorize = await this.authenticationProvider.FindUserAsync(isValidRefreshToken.Result.UserId);
            if (!isAuthorize.IsSuccessful) return BadRequest(new { message = isAuthorize.ResponseMessage });

            var claims = await this.jWTProvider.GetValidClaims(isAuthorize.Result);
            var token = this.jWTProvider.GenerateToken(claims, isValidClient.Result.Url);
            
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