using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class UserNameDto
    {
        [Required]
        public string UserName { get; set; }
    }
}