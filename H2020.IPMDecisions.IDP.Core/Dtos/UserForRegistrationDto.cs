using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class UserForRegistrationDto : UserForAuthenticationDto
    {
        [Required]
        public List<string> UserType { get; set; }
    }
}