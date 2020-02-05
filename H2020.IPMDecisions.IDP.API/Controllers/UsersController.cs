using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.API.Helpers;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "SuperAdmin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            this.userManager = userManager
                ?? throw new System.ArgumentNullException(nameof(userManager));
        }

        [HttpGet("", Name = "GetUsers")]
        [HttpHead]
        // GET: api/users
        public async Task<IActionResult> GetUsers()
        {
            var users = await this.userManager.Users.ToListAsync();
            if (users.Count == 0) return NotFound();

            var usersToReturn = this.mapper.Map<List<UserDto>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet("{userId:guid}", Name = "GetUser")]
        // GET: api/users/1
        public async Task<IActionResult> GetUser([FromRoute] Guid userId)
        {
            var user = await this.userManager.FindByIdAsync(userId.ToString());

            if (user == null) return NotFound();

            var links = CreateLinksForUser(userId);

            var userToReturn = this.mapper.Map<UserDto>(user)
                .ShapeData()
                as IDictionary<string, object>; ;

            userToReturn.Add("links", links);

            return Ok(userToReturn);
        }

        [HttpDelete("{userId:guid}", Name = "DeleteUser")]
        // DELETE: api/users/1
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
        {
            var userToDelete = await this.userManager.FindByIdAsync(userId.ToString());

            if (userToDelete == null) return NotFound();

            var result = await this.userManager.DeleteAsync(userToDelete);

            if (result.Succeeded) return NoContent();

            return BadRequest(result);
        }

        [HttpOptions]
        // OPTIONS: api/users
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST,GET,DELETE");
            return Ok();
        }


        #region Helpers
        private IEnumerable<LinkDto> CreateLinksForUser(
            Guid userId)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                Url.Link("GetUser", new { userId }),
                "self",
                "GET"));

            links.Add(new LinkDto(
                Url.Link("DeleteUser", new { userId }),
                "delete_user",
                "DELETE"));

            links.Add(new LinkDto(
                Url.Link("GetRolesFromUser", new { userId }),
                "roles",
                "GET"));

            links.Add(new LinkDto(
                Url.Link("AssignRolesToUser", new { userId }),
                "assign_roles_to_user",
                "POST"));

            links.Add(new LinkDto(
                Url.Link("RemoveRolesFromUser", new { userId }),
                "remove_roles_to_user",
                "DELETE"));

            links.Add(new LinkDto(
                Url.Link("GetClaimsFromUser", new { userId }),
                "claims",
                "GET"));

            links.Add(new LinkDto(
                Url.Link("AssignClaimsToUser", new { userId }),
                "assign_claims_to_user",
                "POST"));

            links.Add(new LinkDto(
                Url.Link("RemoveClaimsFromUser", new { userId }),
                "remove_claims_to_user",
                "DELETE"));

            return links;
        }
        #endregion
    }
}