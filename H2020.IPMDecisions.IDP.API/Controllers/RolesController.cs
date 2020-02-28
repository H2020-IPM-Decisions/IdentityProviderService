using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using H2020.IPMDecisions.IDP.BLL;
using H2020.IPMDecisions.IDP.Core.Models;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Authorize(Roles = "Admin", AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public RolesController(
            IBusinessLogic businessLogic)
        {           
            this.businessLogic = businessLogic
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces("application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpGet("", Name = "GetRoles")]
        [HttpHead]
        // GET: api/Roles
        public async Task<IActionResult> Get(
            [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await businessLogic.GetRoles(fields, mediaType);

            if (response.IsSuccessful)
            {
                if (response.Result.Count() == 0)
                    return NoContent();

                return Ok(response.Result);
            }

            return BadRequest(new { message = response.ErrorMessage });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces("application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpGet("{id:guid}", Name = "GetRole")]
        // GET: api/Roles/5
        public async Task<IActionResult> Get(
            [FromRoute] Guid id,
            [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await businessLogic.GetRole(id, fields, mediaType);

            if (response.IsSuccessful)
            {
                if (response.Result.Count() == 0)
                    return NoContent();

                return Ok(response.Result);
            }

            return BadRequest(new { message = response.ErrorMessage });
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpPost("", Name = "CreateRole")]
        // POST: api/Roles
        public async Task<IActionResult> Post(
            [FromBody]RoleForCreationDto roleDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await businessLogic.CreateRole(roleDto, mediaType);

            if (response.IsSuccessful)
            {
                var responseAsRoleDto = (GenericResponse<IDictionary<string, object>>)response;
                return CreatedAtRoute("GetRole",
                 new { id = responseAsRoleDto.Result["Id"] },
                 responseAsRoleDto.Result);
            }

            var responseAsIdentityResult = (GenericResponse<IdentityResult>)response;
            return BadRequest(responseAsIdentityResult.Result);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "DeleteRole")]
        // DELETE: api/Roles/5
        public async Task<IActionResult> Delete(Guid id)
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
        // OPTIONS: api/Roles
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST,GET,DELETE");
            return Ok();
        }
    }
}