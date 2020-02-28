using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace H2020.IPMDecisions.IDP.Core.Profiles
{
    public class RolesProfile : MainProfile
    {
        public RolesProfile()
        { 
            // Entities to Dtos
            CreateMap<IdentityRole, RoleDto>();

            // Dtos to Entities
            CreateMap<RoleDto, IdentityRole>();
            CreateMap<RoleForManipulationDto, IdentityRole>(); 
        }


    }
}