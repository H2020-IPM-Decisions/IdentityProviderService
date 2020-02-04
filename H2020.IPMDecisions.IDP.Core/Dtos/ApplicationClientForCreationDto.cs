using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.IDP.Core.Entities;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class ApplicationClientForCreationDto : ApplicationClientForManipulationDto
    {
        [Required(ErrorMessage = "Name is required")]
        public override string Name { get => base.Name; set => base.Name = value; }
        [Required]
        public override ApplicationClientType ApplicationClientType { get => base.ApplicationClientType; set => base.ApplicationClientType = value; }
        [Required]
        public override bool Enabled { get => base.Enabled; set => base.Enabled = value; }
        [Required]
        public override int RefreshTokenLifeTime { get => base.RefreshTokenLifeTime; set => base.RefreshTokenLifeTime = value; }
    }
}