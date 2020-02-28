using System;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.BLL.Providers;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Models;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public Task<GenericResponse<IList<string>>> GetUserRoles(Guid id)
        {
            throw new NotImplementedException();
        }
        
        public Task<GenericResponse<UserDto>> ManageUserRoles(Guid id, List<RoleForManipulationDto> claimsDto, bool remove)
        {
            throw new NotImplementedException();
        }
    }
}