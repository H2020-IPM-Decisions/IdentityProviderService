
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Text;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse> ChangePassword(Guid userId, ChangePasswordDto changePasswordDto)
        {
            var applicationUser = await this.dataService.UserManager.FindByIdAsync(userId.ToString());
            if (applicationUser != null)
            {
                var identityResult = await this.dataService.UserManager.ChangePasswordAsync(applicationUser,
                    changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

                if (identityResult.Succeeded)
                {
                    var successResponse = GenericResponseBuilder.Success<IdentityResult>(identityResult);
                    return successResponse;
                }
                else
                {
                    var noSuccessResponse = GenericResponseBuilder.NoSuccess<IdentityResult>
                        (identityResult, "Password change request unsuccessfull");
                    return noSuccessResponse;
                }
            }
            //User Id is not known, but don't want to report this for security reasons.
            var failResponse = GenericResponseBuilder.NoSuccess<IdentityResult>(null, "Password change request unsuccessfull");
            return failResponse;
        }
    }
}