using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class UserForRegistrationDto : UserForAuthenticationDto
    {
        public string UserType { get; set; }
    }
}