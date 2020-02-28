using System.Collections.Generic;
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
        Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetRoles(string fields, string mediaType);
        #endregion

        #region UserClaims Controller
        #endregion

        #region UserRoles Controller
        #endregion

        #region Users Controller
        #endregion
    }
}