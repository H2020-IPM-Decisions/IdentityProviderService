using System;
using System.Net.Mime;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.API.Filters;
using H2020.IPMDecisions.IDP.BLL;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/users/{userId:guid}/accounts")]
    [ServiceFilter(typeof(UserAccessingOwnDataActionFilter))]
    public class UserAccountsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public UserAccountsController(
            IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("changepassword", Name = "ChangePassword")]
        //POST: api/users/1/Accounts/changepassword
        public async Task<IActionResult> ChangePassword([FromRoute] Guid userId, [FromBody] ChangePasswordDto changePassword)
        {
            var response = await businessLogic.ChangePassword(userId, changePassword);

            if (response.IsSuccessful)
                return Ok();

            var responseAsIdentityResult = (GenericResponse<IdentityResult>)response;
            if (responseAsIdentityResult.Result == null && !string.IsNullOrEmpty(responseAsIdentityResult.ErrorMessage))
                return BadRequest(new { message = response.ErrorMessage });
            if (responseAsIdentityResult.Result == null)
                return NotFound();

            return BadRequest(responseAsIdentityResult.Result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Append("Allow", "OPTIONS,POST");
            return Ok();
        }
    }
}