using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Models;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;

namespace H2020.IPMDecisions.IDP.BLL
{
    public interface IBusinessLogic
    {
        #region Accounts Controller
        Task<GenericResponse> AddNewUser(UserForRegistrationDto user);
        Task<GenericResponse<BearerToken>> AuthenticateUser(UserForAuthenticationDto user, HttpRequest request);
        Task<GenericResponse<BearerToken>> AuthenticateUser(HttpRequest request);
        #endregion

        #region ApplicationClients Controller
        Task<GenericResponse> CreateApplicationClient(ApplicationClientForCreationDto applicationClient, string mediaType);
        Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetApplicationClient(string mediaType);
        Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetApplicationClients(ApplicationClientForCreationDto resourceParameter, string fields, string mediaType);
        Task<GenericResponse> DeleteApplicationClient(Guid id);
        Task<GenericResponse> UpdateApplicationClient(Guid id, JsonPatchDocument<ApplicationClientForUpdateDto> patchDocument);
        #endregion

        #region RefreshTokens Controller
        Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetRefreshToken(string fields, string mediaType);
        Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetRefreshTokens(RefreshTokenResourceParameter resourceParameter, string fields, string mediaType);
        Task<GenericResponse> DeleteRefreshToken(Guid id);
        #endregion

        #region Roles Controller
        Task<GenericResponse> CreateRole(RoleForManipulationDto role, string mediaType);
        Task<GenericResponse> DeleteRole(Guid id);
        Task<GenericResponse<IDictionary<string, object>>> GetRole(Guid id, string fields, string mediaType);
        Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetRoles(string fields, string mediaType);
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
        Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetUser(string fields, string mediaType);
        Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetUsers(ApplicationUserResourceParameter resourceParameter, string fields, string mediaType);
        Task<GenericResponse> DeleteUser(Guid id);
        #endregion
    }
}