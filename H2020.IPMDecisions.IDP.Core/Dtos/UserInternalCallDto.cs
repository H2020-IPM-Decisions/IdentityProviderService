using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class UserInternalCallDto
    {
        public Guid Id { get; set; }
        public IList<Claim> Claims { get; set; }
    }
}