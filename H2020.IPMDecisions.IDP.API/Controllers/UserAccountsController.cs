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
    [Authorize]  //protected by json token
    [Route("api/users/{userId:guid}/accounts")] //user id in URL!
    [ServiceFilter(typeof(UserAccessingOwnDataActionFilter))] //so filter to test user id passed in context matches token user ID
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
        //[Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("changepassword", Name = "ChangePassword")]
        //POST: api/UserAccounts/changepassword
        //Take in new password
        public async Task<IActionResult> ChangePassword([FromRoute] Guid userId,[FromBody] ChangePasswordDto changePassword)
        {
            //UserID in URL
            //BLL to change password           
            var response = await businessLogic.ChangePassword(userId, changePassword);
            if (response.IsSuccessful)
            { 
                //return Ok();
                var responseAsIdentityResult = (GenericResponse<IdentityResult>)response;
                return Ok(responseAsIdentityResult.Result);
            }
            else
            {
                //Need check if this is required
                var responseAsIdentityResult = (GenericResponse<IdentityResult>)response;
                if (responseAsIdentityResult.Result == null) return BadRequest(response);
                    return BadRequest(responseAsIdentityResult.Result);
            } 
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