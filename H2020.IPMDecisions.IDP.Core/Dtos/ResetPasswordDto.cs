using System.ComponentModel.DataAnnotations;
namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class ResetPasswordDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Token { get; set; }
    }
}