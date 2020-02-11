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
using Microsoft.EntityFrameworkCore;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Core.Services;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    [Route("/api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IDataService dataService;
        private readonly IMapper mapper;
        private readonly IPropertyCheckerService propertyCheckerService;

        public RolesController(
            IDataService dataService,
            IMapper mapper,
            IPropertyCheckerService propertyCheckerService)
        {
            this.dataService = dataService 
                ?? throw new ArgumentNullException(nameof(dataService));
            this.mapper = mapper
                ?? throw new System.ArgumentNullException(nameof(mapper));
            this.propertyCheckerService = propertyCheckerService 
                ?? throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [HttpGet("", Name = "GetRoles")]
        [HttpHead]
        // GET: api/Roles
        public async Task<IActionResult> Get([FromQuery] string fields)
        {
            if (!propertyCheckerService.TypeHasProperties<RoleDto>(fields, true))
            {
                return BadRequest();
            }
            var roles = await this.dataService.RoleManager.Roles.ToListAsync();
            if (roles.Count == 0) return NotFound();

            var rolesToReturn = this.mapper
                .Map<IEnumerable<RoleDto>>(roles)
                .ShapeData(fields);

            var rolesToReturnWithLinks = rolesToReturn.Select(role =>
            {
                var userAsDictionary = role as IDictionary<string, object>;
                var userLinks = CreateLinksForRole((Guid)userAsDictionary["Id"]);
                userAsDictionary.Add("links", userLinks);
                return userAsDictionary;
            });

            return Ok(rolesToReturnWithLinks);
        }

        [HttpGet("{roleId:guid}", Name = "GetRole")]
        // GET: api/Roles/5
        public async Task<IActionResult> Get(Guid roleId, [FromQuery] string fields)
        {
            if (!propertyCheckerService.TypeHasProperties<RoleDto>(fields))
            {
                return BadRequest();
            }
            var roleEntity = await this.dataService.RoleManager.FindByIdAsync(roleId.ToString());

            if (roleEntity == null) return NotFound();

            var links = CreateLinksForRole(roleId);
            var roleToReturn = this.mapper.Map<RoleDto>(roleEntity)
                .ShapeData(fields)
                as IDictionary<string, object>;
            roleToReturn.Add("links", links);

            return Ok(roleToReturn);
        }

        [HttpPost("", Name = "CreateRole")]
        // POST: api/Roles
        public async Task<IActionResult> Post([FromBody]RoleForCreationDto roleDto, [FromQuery] string fields)
        {
            if (!propertyCheckerService.TypeHasProperties<RoleDto>(fields, true))
            {
                return BadRequest();
            }

            var roleEntity = this.mapper.Map<IdentityRole>(roleDto);
            var result = await this.dataService.RoleManager.CreateAsync(roleEntity);

            if (result.Succeeded)
            {
                var links = CreateLinksForRole(Guid.Parse(roleEntity.Id));

                var roleToReturn = this.mapper.Map<RoleDto>(roleEntity)
                    .ShapeData(fields)
                    as IDictionary<string, object>; ;

                roleToReturn.Add("links", links);

                return CreatedAtRoute("GetRole",
                 new { roleId = roleToReturn["Id"] },
                 roleToReturn);
            }
            return BadRequest(result);
        }

        [HttpDelete("{roleId:guid}", Name = "DeleteRole")]
        // DELETE: api/Roles/5
        public async Task<IActionResult> Delete(Guid roleId)
        {
            var roleEntity = await this.dataService.RoleManager.FindByIdAsync(roleId.ToString());
            if (roleEntity == null) return NotFound();

            var result = await this.dataService.RoleManager.DeleteAsync(roleEntity);
            if (result.Succeeded) return NoContent();

            return BadRequest(result);
        }

        [HttpOptions]
        // OPTIONS: api/Roles
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST,GET,DELETE");
            return Ok();
        }

        #region helpers
        private IEnumerable<LinkDto> CreateLinksForRole(
            Guid roleId)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                Url.Link("GetRole", new { roleId }),
                "self",
                "GET"));


            links.Add(new LinkDto(
                Url.Link("DeleteRole", new { roleId }),
                "delete_role",
                "DELETE"));

            return links;
        }
        #endregion
    }
}