using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.IDP.Core.Entities;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class ApplicationClientForCreationDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name max length 100 characters")]
        public string Name { get; set; }
        [Required]
        public ApplicationClientType ApplicationClientType { get; set; }
        [Required]
        public bool Enabled { get; set; }
        [Required]
        public int RefreshTokenLifeTime { get; set; }
    }
}