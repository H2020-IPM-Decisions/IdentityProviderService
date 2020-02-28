using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IList<Claim>>> GetUserClaims(Guid id)
        {
            var user = await this.dataService.UserManager.FindByIdAsync(id.ToString());
            if (user == null) return GenericResponseBuilder.Success<IList<Claim>>(null);

            var claimsToReturn = await this.dataService.UserManager.GetClaimsAsync(user);
            if (claimsToReturn.Count == 0) return GenericResponseBuilder.Success<IList<Claim>>(null);

            return GenericResponseBuilder.Success<IList<Claim>>(claimsToReturn);
        }

        public async Task<GenericResponse<UserDto>> ManageUserClaims(Guid id, List<ClaimForManipulationDto> claimsDto, bool remove = false)
        {
            var user = await this.dataService.UserManager.FindByIdAsync(id.ToString());
            if (user == null) GenericResponseBuilder.Success();

            var currentUserClaims = await this.dataService.UserManager.GetClaimsAsync(user);

            foreach (var claim in claimsDto)
            {
                if (!currentUserClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value) & !remove)
                {
                    await this.dataService.UserManager.AddClaimAsync(user, CreateClaim(claim.Type, claim.Value));                   
                }
                else if(currentUserClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value) & remove)
                {
                    await this.dataService.UserManager.RemoveClaimAsync(user, CreateClaim(claim.Type, claim.Value));
                }
            }
            return GenericResponseBuilder.Success<UserDto>(this.mapper.Map<UserDto>(user));
        }

        #region helpers
        private static Claim CreateClaim(string type, string value)
        {
            return new Claim(type, value, ClaimValueTypes.String);
        }
        #endregion

    }
}