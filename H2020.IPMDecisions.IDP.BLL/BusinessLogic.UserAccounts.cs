
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse> ChangePassword(Guid userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var applicationUser = await this.dataService.UserManager.FindByIdAsync(userId.ToString());

                if (applicationUser == null) 
                    return GenericResponseBuilder.NoSuccess<IdentityResult>(null);

                var identityResult = await this.dataService.UserManager.ChangePasswordAsync(
                    applicationUser, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

                if (!identityResult.Succeeded)
                    return GenericResponseBuilder.NoSuccess<IdentityResult>(identityResult, this.jsonStringLocalizer["accounts.change_password_error"].ToString());

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ChangePassword. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess<IdentityResult>(null, ex.Message.ToString());
            }
        }
    }
}