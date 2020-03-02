using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public Task<GenericResponse> CreateApplicationClient(ApplicationClientForCreationDto applicationClient, string mediaType)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse> DeleteApplicationClient(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetApplicationClient(string mediaType)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetApplicationClients(ApplicationClientForCreationDto resourceParameter, string fields, string mediaType)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse> UpdateApplicationClient(Guid id, JsonPatchDocument<ApplicationClientForUpdateDto> patchDocument)
        {
            throw new NotImplementedException();
        }
    }
}