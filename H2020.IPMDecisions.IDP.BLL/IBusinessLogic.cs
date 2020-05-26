using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Models;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;

namespace H2020.IPMDecisions.IDP.BLL
{
    public interface IBusinessLogic
    {
        #region Accounts Controller
        Task<GenericResponse> AddNewUser(UserForRegistrationDto user);
        Task<GenericResponse<AuthenticationDto>> AuthenticateUser(UserForAuthenticationDto user, HttpRequest request);
        Task<GenericResponse<AuthenticationDto>> AuthenticateUser(HttpRequest request);
        Task<GenericResponse> ConfirmEmail(Guid userId, string token);
        Task<GenericResponse> ResetPasswordEmail(string userName);
        Task<GenericResponse> ResetPassword(ResetPasswordDto resetPasswordDto);
        #endregion

        #region ApplicationClients Controller
        Task<GenericResponse<IDictionary<string, object>>> CreateApplicationClient(ApplicationClientForCreationDto applicationClient, string mediaType);
        Task<GenericResponse<ApplicationClientDto>> CreateApplicationClient(Guid id, ApplicationClient applicationClient);
        Task<GenericResponse> DeleteApplicationClient(Guid id);
        Task<GenericResponse<ApplicationClient>> GetApplicationClient(Guid id);
        Task<GenericResponse<IDictionary<string, object>>> GetApplicationClient(Guid id, string fields, string mediaType);
        Task<GenericResponse<ShapedDataWithLinks>> GetApplicationClients(ApplicationClientResourceParameter resourceParameter, string mediaType);
        ApplicationClient MapToApplicationClientAddingClientSecret(ApplicationClientForUpdateDto applicationClient);
        ApplicationClientForUpdateDto MapToApplicationClientForUpdateDto(ApplicationClient applicationClient);
        Task<GenericResponse> UpdateApplicationClient(ApplicationClient applicationClient, ApplicationClientForUpdateDto applicationClientForUpdate);
        #endregion

        #region RefreshTokens Controller
        Task<GenericResponse<IDictionary<string, object>>> GetRefreshToken(Guid id, string fields, string mediaType);
        Task<GenericResponse<ShapedDataWithLinks>> GetRefreshTokens(RefreshTokenResourceParameter resourceParameter, string mediaType);
        Task<GenericResponse> DeleteRefreshToken(Guid id);
        #endregion

        #region Roles Controller
        Task<GenericResponse> CreateRole(RoleForManipulationDto role, string mediaType);
        Task<GenericResponse> DeleteRole(Guid id);
        Task<GenericResponse<IDictionary<string, object>>> GetRole(Guid id, string fields, string mediaType);
        Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetRoles(string fields, string mediaType);
        #endregion

        #region UserAccounts Controller
        Task<GenericResponse> ChangePassword(Guid userId, ChangePasswordDto changePasswordDto);
        #endregion

        #region UserClaims Controller
        Task<GenericResponse<IList<Claim>>> GetUserClaims(Guid id);
        Task<GenericResponse<UserDto>> ManageUserClaims(Guid id, List<ClaimForManipulationDto> claims, bool remove = false);
        #endregion

        #region UserRoles Controller
        Task<GenericResponse<IList<string>>> GetUserRoles(Guid id);
        Task<GenericResponse<UserDto>> ManageUserRoles(Guid id, List<RoleForManipulationDto> roles, bool remove = false);
        #endregion

        #region Users Controller
        Task<GenericResponse<IDictionary<string, object>>> GetUser(Guid id, string fields, string mediaType);
        Task<GenericResponse<ShapedDataWithLinks>> GetUsers(ApplicationUserResourceParameter resourceParameter, string mediaType);
        Task<GenericResponse> DeleteUser(Guid id);
        #endregion
    }
}