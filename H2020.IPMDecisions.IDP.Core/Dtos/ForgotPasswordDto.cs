using System.ComponentModel.DataAnnotations;
namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}