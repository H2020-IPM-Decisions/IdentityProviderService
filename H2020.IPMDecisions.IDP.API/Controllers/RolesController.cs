using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]

    [Route("/api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;

        public RolesController(
            RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            this.roleManager = roleManager
                ?? throw new ArgumentNullException(nameof(roleManager));
            this.mapper = mapper
                ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        [HttpGet("", Name = "GetRoles")]
        [HttpHead]
        // GET: api/Roles
        public async Task<IActionResult> Get()
        {
            var roles = await this.roleManager.Roles.ToListAsync();
            if (roles.Count == 0) return NotFound();

            var rolesToReturn = this.mapper.Map<List<RoleDto>>(roles);
            return Ok(rolesToReturn);
        }

        [HttpGet("{id:guid}", Name = "GetRoleById")]
        // GET: api/Roles/5
        public async Task<IActionResult> Get(Guid id)
        {
            var roleEntity = await this.roleManager.FindByIdAsync(id.ToString());

            if (roleEntity == null) return NotFound();
            var roleToReturn = this.mapper.Map<RoleDto>(roleEntity);

            return Ok(roleToReturn);
        }

        [HttpPost("", Name = "CreateRole")]
        // POST: api/Roles
        public async Task<IActionResult> Post([FromBody]RoleForCreationDto roleDto)
        {

            var roleEntity = this.mapper.Map<IdentityRole>(roleDto);
            var result = await this.roleManager.CreateAsync(roleEntity);

            if (result.Succeeded)
            {
                var roleToReturn = this.mapper.Map<RoleDto>(roleEntity);

                return CreatedAtRoute("GetRoleById",
                 new { id = roleEntity.Id}, 
                 roleToReturn);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id:guid}", Name = "DeleteRole")]
        // DELETE: api/Roles/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var roleEntity = await this.roleManager.FindByIdAsync(id.ToString());
            if (roleEntity == null) return NotFound();

            var result = await this.roleManager.DeleteAsync(roleEntity);
            if (result.Succeeded) return NoContent();

            return BadRequest(result);
        }

    }
}