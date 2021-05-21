using System;
using Microsoft.AspNetCore.Identity;

namespace H2020.IPMDecisions.IDP.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime RegistrationDate { get; set; }
        public DateTime LastValidAccess { get; set; }
    }
}