using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.AspNetCore.Http;

namespace H2020.IPMDecisions.IDP.BLL
{
    public interface IBusinessLogic
    {
        Task<GenericResponse> AddNewUser(UserForRegistrationDto user);
        Task<GenericResponse<BearerToken>> AuthenticateUser(UserForAuthenticationDto user, HttpRequest request);
        Task<GenericResponse<BearerToken>> AuthenticateUser(HttpRequest request);
    }
}