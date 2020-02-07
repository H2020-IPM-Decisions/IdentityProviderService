using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/users/{userId:guid}/claims")]
    [Authorize(Roles = "SuperAdmin")]
    public class UserClaimsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public UserClaimsController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            this.userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("", Name = "AssignClaimsToUser")]
        // POST: api/users/1/Claims
        public async Task<IActionResult> Post(
            [FromRoute] Guid userId,
            [FromBody] List<ClaimForCreationDto> claimsDto)
        {
            var user = await this.userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            var currentUserClaims = await this.userManager.GetClaimsAsync(user);

            foreach (var claim in claimsDto)
            {
                if (!currentUserClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                {
                    await this.userManager.AddClaimAsync(user, CreateClaim(claim.Type, claim.Value));
                }
            }

            var userToReturn = this.mapper.Map<UserDto>(user);
            return Ok(userToReturn);
        }

        [HttpDelete("", Name = "RemoveClaimsFromUser")]
        // DELETE: api/users/1/Claims
        public async Task<IActionResult> Delete(
            [FromRoute] Guid userId,
            [FromBody] List<ClaimForDeletionDto> claimsDto)
        {
            var user = await this.userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            foreach (var claim in claimsDto)
            {
                await this.userManager.RemoveClaimAsync(user, CreateClaim(claim.Type, claim.Value));
            }
            var userToReturn = this.mapper.Map<UserDto>(user);
            return Ok(userToReturn);
        }

        [HttpGet("", Name = "GetClaimsFromUser")]
        // GET: api/users/1/Claims
        public async Task<IActionResult> Get(
            [FromRoute] Guid userId)
        {
            var user = await this.userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            var claimsToReturn = await this.userManager.GetClaimsAsync(user);
            if (claimsToReturn.Count == 0) return NotFound();

            return Ok(claimsToReturn);
        }

        [HttpOptions]
        // OPTIONS: api/users/1/Claims
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST,GET,DELETE");
            return Ok();
        }

        #region helpers
        private static Claim CreateClaim(string type, string value)
        {
            return new Claim(type, value, ClaimValueTypes.String);
        }
        #endregion
    }
}