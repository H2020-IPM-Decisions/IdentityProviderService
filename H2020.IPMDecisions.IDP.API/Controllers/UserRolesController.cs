using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.BLL;
using H2020.IPMDecisions.IDP.Core.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/users/{userId:guid}/roles")]
    [Authorize(Roles = "Admin", AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    public class UserRolesController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public UserRolesController(
            IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("", Name = "AssignRolesToUser")]
        // POST: api/users/1/roles
        public async Task<IActionResult> Post(
            [FromRoute] Guid userId,
            [FromBody] List<RoleForManipulationDto> rolesDto)
        {
            var response = await businessLogic.ManageUserRoles(userId, rolesDto);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            if (response.Result == null)
                return NotFound();

            return CreatedAtRoute("GetRolesFromUser",
                    new { userId = response.Result.Id },
                    response.Result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("", Name = "RemoveRolesFromUser")]
        // DELETE: api/users/1/roles
        public async Task<IActionResult> Delete(
            [FromRoute] Guid userId,
            [FromBody] List<RoleForManipulationDto> rolesDto)
        {
            var response = await businessLogic.ManageUserRoles(userId, rolesDto, true);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            if (response.Result == null)
                return NotFound();

            return Ok(response.Result);
        }

        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("", Name = "GetRolesFromUser")]
        // GET: api/users/1/roles
        public async Task<IActionResult> Get(
                [FromRoute] Guid userId)
        {
            var response = await businessLogic.GetUserRoles(userId);

            if (!response.IsSuccessful)
                return BadRequest(response.ErrorMessage);

            if (response.Result == null)
                return NotFound();

            return Ok(response.Result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        // OPTIONS: api/users/1/roles
        public IActionResult Options()
        {
            Response.Headers.Append("Allow", "OPTIONS,POST,GET,DELETE");
            return Ok();
        }
    }
}