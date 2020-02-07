using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/users/{userId:guid}/claims")]
    [Authorize(Roles = "SuperAdmin")]
    public class UserClaimsController : ControllerBase
    {
        private readonly IDataService dataService;
        private readonly IMapper mapper;

        public UserClaimsController(
            IDataService dataService,
            IMapper mapper)
        {
            this.dataService = dataService 
                ?? throw new ArgumentNullException(nameof(dataService));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("", Name = "AssignClaimsToUser")]
        // POST: api/users/1/Claims
        public async Task<IActionResult> Post(
            [FromRoute] Guid userId,
            [FromBody] List<ClaimForCreationDto> claimsDto)
        {
            var user = await this.dataService.UserManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            var currentUserClaims = await this.dataService.UserManager.GetClaimsAsync(user);

            foreach (var claim in claimsDto)
            {
                if (!currentUserClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                {
                    await this.dataService.UserManager.AddClaimAsync(user, CreateClaim(claim.Type, claim.Value));
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
            var user = await this.dataService.UserManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            foreach (var claim in claimsDto)
            {
                await this.dataService.UserManager.RemoveClaimAsync(user, CreateClaim(claim.Type, claim.Value));
            }
            var userToReturn = this.mapper.Map<UserDto>(user);
            return Ok(userToReturn);
        }

        [HttpGet("", Name = "GetClaimsFromUser")]
        // GET: api/users/1/Claims
        public async Task<IActionResult> Get(
            [FromRoute] Guid userId)
        {
            var user = await this.dataService.UserManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            var claimsToReturn = await this.dataService.UserManager.GetClaimsAsync(user);
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