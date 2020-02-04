using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/user/{userId:guid}/roles")]
    public class UserRolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public UserRolesController(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            this.userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            this.roleManager = roleManager
             ?? throw new ArgumentNullException(nameof(roleManager));
        }

        [HttpPost("", Name = "AssignRolesToUser")]
        public async Task<IActionResult> Post(
            [FromRoute] Guid userId,
            [FromBody] List<RoleForCreationDto> rolesDto)
        {
            var user = await this.userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            foreach (var role in rolesDto)
            {
                var roleEntity = await this.roleManager.FindByNameAsync(role.Name);

                if (roleEntity == null)
                {
                    roleEntity = this.mapper.Map<IdentityRole>(role);
                    await this.roleManager.CreateAsync(roleEntity);                    
                };
                await this.userManager.AddToRoleAsync(user, roleEntity.Name);               
            }

            var userToReturn = this.mapper.Map<UserDto>(user);
            return Ok(userToReturn);
        }


        [HttpDelete("", Name = "RemoveRolesFromUser")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid userId,
            [FromBody] List<RoleForDeletionDto> rolesDto)
        {
            var user = await this.userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            foreach (var role in rolesDto)
            {
                var roleEntity = await this.roleManager.FindByNameAsync(role.Name);

                if (roleEntity != null)
                {
                    await this.userManager.RemoveFromRoleAsync(user, roleEntity.Name);
                };                
            }
            var userToReturn = this.mapper.Map<UserDto>(user);
            return Ok(userToReturn);
        }

        [HttpGet("", Name = "GetRolesFromUser")]
        public async Task<IActionResult> Get(
            [FromRoute] Guid userId)
        {
            var user = await this.userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            var rolesToReturn = await this.userManager.GetRolesAsync(user);
            return Ok(rolesToReturn);
        }

        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST,GET,DELETE");
            return Ok();
        }
    }
}