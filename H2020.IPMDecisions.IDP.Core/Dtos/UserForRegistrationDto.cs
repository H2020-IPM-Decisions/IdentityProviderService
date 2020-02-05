using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class UserForRegistrationDto : UserForAuthenticationDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}