using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/users")]
    public class Users : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public Users(
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
            if (users.Count() == 0) return NotFound();

            var userToReturn = this.mapper.Map<List<UserDto>>(users);

            return Ok(userToReturn);
        }

        [HttpGet("{userId:guid}", Name = "GetUserById")]
        // GET: api/users/1
        public async Task<IActionResult> GetUser([FromRoute] Guid userId)
        {
            var user = await this.userManager.FindByIdAsync(userId.ToString());

            if (user == null) return NotFound();

            var userToReturn = this.mapper.Map<UserDto>(user);

            // Forbid HATEOS implementation
            // var links = CreateLinksForUser(userToReturn.Id);

            return Ok(userToReturn);
        }

        [HttpDelete("{userId:guid}", Name = "DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
        {
            var userToDelete = await this.userManager.FindByIdAsync(userId.ToString());

            if (userToDelete == null) return NotFound();

            var result = await this.userManager.DeleteAsync(userToDelete);

            if (result.Succeeded) return NoContent();

            return BadRequest(result);
        }


        #region Helpers
        private IEnumerable<LinkDto> CreateLinksForUser(
            Guid authorId)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                Url.Link("GetAuthor", new { authorId }),
                "self",
                "GET"));


            links.Add(new LinkDto(
                    Url.Link("DeleteAuthor", new { authorId }),
                    "delete_author",
                    "DELETE"));

            return links;
        }
        #endregion
    }
}