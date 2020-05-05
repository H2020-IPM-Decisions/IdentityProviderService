using System.Collections.Generic;
using System.Security.Claims;

namespace H2020.IPMDecisions.IDP.Core.Interfaces
{
    public interface IUserWithRolesClaimsDto : IUserDto
    {
        IList<string> Roles { get; set; }
        IList<Claim> Claims { get; set; }
    }
}