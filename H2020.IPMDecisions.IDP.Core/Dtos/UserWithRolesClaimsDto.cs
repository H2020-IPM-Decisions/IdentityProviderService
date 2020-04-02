using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class UserWithRolesClaimsDto : UserDto
    {
        public IList<string> Roles { get; set; }
        public IList<Claim> Claims { get; set; }
    }
}