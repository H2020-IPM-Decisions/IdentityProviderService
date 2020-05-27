using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Models
{
    public class Email
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string ToAddress { get; set; }
    }
}