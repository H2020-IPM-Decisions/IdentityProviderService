using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Models;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public Task<GenericResponse> DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetUser(string fields, string mediaType)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetUsers(ApplicationUserResourceParameter resourceParameter, string fields, string mediaType)
        {
            throw new NotImplementedException();
        }
    }
}