using System.Collections.Generic;
using System.Security.Claims;
using H2020.IPMDecisions.IDP.Core.Interfaces;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class UserWithRolesClaimsDto : UserDto, IUserWithRolesClaimsDto
    {
        public IList<string> Roles { get; set; }
        public IList<Claim> Claims { get; set; }
    }
}