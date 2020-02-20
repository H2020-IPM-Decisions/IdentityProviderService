using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("api/users/{userId:guid}/roles")]
    [Authorize(Roles = "Admin", AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    public class UserRolesController : ControllerBase
    {
        private readonly IDataService dataService;
        private readonly IMapper mapper;

        public UserRolesController(
            IDataService dataService,
            IMapper mapper)
        {
            this.dataService = dataService 
                ?? throw new ArgumentNullException(nameof(dataService));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("", Name = "AssignRolesToUser")]
        // POST: api/users/1/roles
        public async Task<IActionResult> Post(
            [FromRoute] Guid userId,
            [FromBody] List<RoleForCreationDto> rolesDto)
        {
            var user = await this.dataService.UserManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            foreach (var role in rolesDto)
            {
                var roleEntity = await this.dataService.RoleManager.FindByNameAsync(role.Name);

                if (roleEntity == null)
                {
                    roleEntity = this.mapper.Map<IdentityRole>(role);
                    await this.dataService.RoleManager.CreateAsync(roleEntity);
                };
                await this.dataService.UserManager.AddToRoleAsync(user, roleEntity.Name);
            }

            var userToReturn = this.mapper.Map<UserDto>(user);
            return CreatedAtRoute("GetRolesFromUser",
                    new { userId = userToReturn.Id },
                    userToReturn);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("", Name = "RemoveRolesFromUser")]
        // DELETE: api/users/1/roles
        public async Task<IActionResult> Delete(
            [FromRoute] Guid userId,
            [FromBody] List<RoleForDeletionDto> rolesDto)
        {
            var user = await this.dataService.UserManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            foreach (var role in rolesDto)
            {
                var roleEntity = await this.dataService.RoleManager.FindByNameAsync(role.Name);

                if (roleEntity != null)
                {
                    await this.dataService.UserManager.RemoveFromRoleAsync(user, roleEntity.Name);
                };
            }
            var userToReturn = this.mapper.Map<UserDto>(user);
            return Ok(userToReturn);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("", Name = "GetRolesFromUser")]
        // GET: api/users/1/roles
        public async Task<IActionResult> Get(
            [FromRoute] Guid userId)
        {
            var user = await this.dataService.UserManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            var rolesToReturn = await this.dataService.UserManager.GetRolesAsync(user);
            if (rolesToReturn.Count == 0) return NotFound();

            return Ok(rolesToReturn);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        // OPTIONS: api/users/1/roles
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST,GET,DELETE");
            return Ok();
        }
    }
}