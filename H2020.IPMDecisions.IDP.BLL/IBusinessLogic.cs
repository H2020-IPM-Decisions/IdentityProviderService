using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.AspNetCore.Http;

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
        #endregion

        #region RefreshTokens Controller
        #endregion

        #region Roles Controller
        Task<GenericResponse> CreateRole(RoleForManipulationDto role, string mediaType);
        Task<GenericResponse> DeleteRole(Guid id);
        Task<GenericResponse<IDictionary<string, object>>> GetRole(Guid id, string fields, string mediaType);
        Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetRoles(string fields, string mediaType);
        #endregion

        #region UserClaims Controller
        Task<GenericResponse<IList<Claim>>> GetUserClaims(Guid id);
        Task<GenericResponse<UserDto>> ManageUserClaims(Guid id, List<ClaimForManipulationDto> claimsDto, bool remove = false);
        #endregion

        #region UserRoles Controller
        Task<GenericResponse<IList<string>>> GetUserRoles(Guid id);
        Task<GenericResponse<UserDto>> ManageUserRoles(Guid id, List<RoleForManipulationDto> claimsDto, bool remove);

        #endregion

        #region Users Controller
        #endregion
    }
}