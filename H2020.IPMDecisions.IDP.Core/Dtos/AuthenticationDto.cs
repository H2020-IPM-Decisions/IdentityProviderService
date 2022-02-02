using System;
using System.Collections.Generic;
using System.Security.Claims;
using H2020.IPMDecisions.IDP.Core.Interfaces;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class AuthenticationDto : IUserWithRolesClaimsDto, IBearerToken
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
        public IList<Claim> Claims { get; set; }
        public string Token { get; set; }
        public string TokenType => "Bearer";
        public string RefreshToken { get; set; }
        public bool HasDss { get; set; } = false;
    }
}