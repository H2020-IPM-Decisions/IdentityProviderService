using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using System.Text.Json;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Mime;
using H2020.IPMDecisions.IDP.BLL;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin", AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public UsersController(
            IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpGet("", Name = "GetUsers")]
        [HttpHead]
        // GET: api/users
        public async Task<IActionResult> GetUsers(
            [FromQuery] ApplicationUserResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetUsers(resourceParameter, mediaType);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            if (response.Result == null)
                return NotFound();

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(response.Result.PaginationMetaData));

            return Ok(
                new
                {
                    value = response.Result.Value,
                    links = response.Result.Links
                }
            );
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpGet("{id:guid}", Name = "GetUser")]
        // GET: api/users/1
        public async Task<IActionResult> GetUser(
            [FromRoute] Guid id,
            [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetUser(id, fields, mediaType);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            if (response.Result == null)
                return NotFound();

            return Ok(response.Result);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:guid}", Name = "DeleteUser")]
        // DELETE: api/users/1
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            var response = await businessLogic.DeleteRole(id);

            if (response.IsSuccessful)
            {
                return NoContent();
            }

            var responseAsIdentityResult = (GenericResponse<IdentityResult>)response;
            return BadRequest(responseAsIdentityResult.Result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        // OPTIONS: api/users
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST,GET,DELETE");
            return Ok();
        }        
    }
}