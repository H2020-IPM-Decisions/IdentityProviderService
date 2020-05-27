using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Models
{
    public class ForgotPasswordEmail : Email
    {
        [Required]
        public string ForgotPasswordUrl { get; set; }
    }
}