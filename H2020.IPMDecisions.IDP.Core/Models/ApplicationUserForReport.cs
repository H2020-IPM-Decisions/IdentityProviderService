using System;
namespace H2020.IPMDecisions.IDP.Core.Models
{
    public class ApplicationUserForReport
    {
        public string FirstCharactersUserId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastValidAccess { get; set; }
        public string UserType { get; set; }
    }
}