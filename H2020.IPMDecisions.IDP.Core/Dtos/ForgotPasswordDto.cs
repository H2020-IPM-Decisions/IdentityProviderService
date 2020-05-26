using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class ForgotPasswordDto
    {
        [Required]
        public string UserName { get; set; }
    }
}