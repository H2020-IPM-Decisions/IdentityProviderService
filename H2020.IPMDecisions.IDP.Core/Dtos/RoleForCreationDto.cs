using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class RoleForCreationDto
    {
        [Required]
        public string Name { get; set; }
    }
}