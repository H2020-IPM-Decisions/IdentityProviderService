using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Helpers;
using H2020.IPMDecisions.IDP.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Core.Services;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using H2020.IPMDecisions.IDP.BLL;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Authorize(Roles = "Admin", AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IDataService dataService;
        private readonly IMapper mapper;
        private readonly IPropertyCheckerService propertyCheckerService;
        private readonly IBusinessLogic businessLogic;

        public RolesController(
            IDataService dataService,
            IMapper mapper,
            IPropertyCheckerService propertyCheckerService,
            IBusinessLogic businessLogic)
        {
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
            this.mapper = mapper
                ?? throw new System.ArgumentNullException(nameof(mapper));
            this.propertyCheckerService = propertyCheckerService
                ?? throw new ArgumentNullException(nameof(propertyCheckerService));
            this.businessLogic = businessLogic
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            if (!MediaTypeHeaderValue.TryParse(mediaType,
            out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }
            if (!propertyCheckerService.TypeHasProperties<RoleDto>(fields))
                return BadRequest();

            var roleEntity = await this.dataService.RoleManager.FindByIdAsync(id.ToString());

            if (roleEntity == null) return NotFound();

            var roleToReturn = this.mapper.Map<RoleDto>(roleEntity)
                .ShapeData(fields)
                as IDictionary<string, object>;

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                             .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            if (includeLinks)
            {
                var links = CreateLinksForRole(id, fields);
                roleToReturn.Add("links", links);
            }

            return Ok(roleToReturn);
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
            if (!MediaTypeHeaderValue.TryParse(mediaType,
                out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }
            var roleEntity = this.mapper.Map<IdentityRole>(roleDto);
            var result = await this.dataService.RoleManager.CreateAsync(roleEntity);

            if (result.Succeeded)
            {
                var roleToReturn = this.mapper.Map<RoleDto>(roleEntity)
                    .ShapeData()
                    as IDictionary<string, object>; ;

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

                if (includeLinks)
                {
                    var links = CreateLinksForRole(Guid.Parse(roleEntity.Id));
                    roleToReturn.Add("links", links);
                }

                return CreatedAtRoute("GetRole",
                 new { id = roleToReturn["Id"] },
                 roleToReturn);
            }
            return BadRequest(result);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:guid}", Name = "DeleteRole")]
        // DELETE: api/Roles/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var roleEntity = await this.dataService.RoleManager.FindByIdAsync(id.ToString());
            if (roleEntity == null) return NotFound();

            var result = await this.dataService.RoleManager.DeleteAsync(roleEntity);
            if (result.Succeeded) return NoContent();

            return BadRequest(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        // OPTIONS: api/Roles
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST,GET,DELETE");
            return Ok();
        }

        #region helpers
        private IEnumerable<LinkDto> CreateLinksForRole(
            Guid id,
            string fields = "")
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(
                    Url.Link("GetRole", new { id }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                    Url.Link("GetRole", new { id, fields }),
                    "self",
                    "GET"));
            }

            links.Add(new LinkDto(
                Url.Link("DeleteRole", new { id }),
                "delete_role",
                "DELETE"));

            return links;
        }
        #endregion
    }
}