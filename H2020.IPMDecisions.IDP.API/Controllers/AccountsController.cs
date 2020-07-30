using System;
using System.Net.Mime;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.API.Filters;
using H2020.IPMDecisions.IDP.BLL;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public AccountsController(
            IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic ??
                throw new ArgumentNullException(nameof(businessLogic));
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("forgotpassword", Name = "ForgotPassword")]
        // POST: api/Accounts/forgotpassword
        public async Task<IActionResult> ForgotPassword(
            [FromBody] UserEmailDto userEmailDto)
        {
            var response = await businessLogic.ForgotPassword(userEmailDto);

            if (response.IsSuccessful)
                return Ok();

            return BadRequest(new { message = response.ErrorMessage });
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("resetpassword", Name = "ResetPassword")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordDto resetPasswordDto)
        {
            var response = await businessLogic.ResetPassword(resetPasswordDto);

            if (response.IsSuccessful)
                return Ok();

            var responseAsIdentityResult = (GenericResponse<IdentityResult>)response;
            if (responseAsIdentityResult.Result == null) return BadRequest(new { message = response.ErrorMessage });
            return BadRequest(responseAsIdentityResult.Result);
        }

        [ServiceFilter(typeof(IsValidUserClaimValueActionFilter))]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("register", Name = "RegisterUser")]
        // POST: api/Accounts/register
        public async Task<IActionResult> Register([FromBody] UserForRegistrationDto userForRegistration)
        {
            var response = await businessLogic.AddNewUser(userForRegistration);
            if (response.IsSuccessful)
            {
                var responseAsUser = (GenericResponse<UserDto>)response;
                return Ok(responseAsUser.Result);
            }

            var responseAsIdentityResult = (GenericResponse<IdentityResult>)response;
            if (responseAsIdentityResult.Result == null) return BadRequest(response);
            return BadRequest(responseAsIdentityResult.Result);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("authenticate", Name = "AuthenticateUser")]
        [RequiredClientHeader("client_id", "client_secret", "grant_type")]
        // POST: api/Accounts/authenticate
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto userDto)
        {
            var tokenResponse = await businessLogic.AuthenticateUser(userDto, Request);

            if (tokenResponse.IsSuccessful)
                return Ok(tokenResponse.Result);

            return BadRequest(new { message = tokenResponse.ErrorMessage });
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("authenticate/token", Name = "AuthenticateUserWithToken")]
        [RequiredClientHeader("client_id", "client_secret", "grant_type", "refresh_token")]
        // POST: api/Accounts/authenticate/token
        public async Task<IActionResult> AuthenticateToken()
        {
            var tokenResponse = await businessLogic.AuthenticateUser(Request);

            if (tokenResponse.IsSuccessful)
                return Ok(tokenResponse.Result);

            return BadRequest(new { message = tokenResponse.ErrorMessage });
        }

        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("confirmemail", Name = "ConfirmEmail")]
        // GET: api/Accounts/ConfirmEmail
        public async Task<IActionResult> ConfirmEmail([RequiredFromQuery] Guid userId, [RequiredFromQuery] string token)
        {
            var response = await businessLogic.ConfirmEmail(userId, token);

            if (response.IsSuccessful)
                return Ok();

            var responseAsIdentityResult = (GenericResponse<IdentityResult>)response;
            if (responseAsIdentityResult.Result == null) return BadRequest(new { message = response.ErrorMessage });
            return BadRequest(responseAsIdentityResult.Result);
        }

        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("resendConfirmationEmail", Name = "api.accounts.post.resendconfirmationemail")]
        //POST: api/Accounts/resendConfirmationEmail
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] UserEmailDto userEmail)
        {
            var response = await businessLogic.ResendConfirmationEmail(userEmail);

            if (response.IsSuccessful)
                return Ok();
            
            return BadRequest(new { message = response.ErrorMessage });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
    }
}