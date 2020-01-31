using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class UserForRegistrationDto : UserForAuthentificationDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}