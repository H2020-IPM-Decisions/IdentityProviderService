using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class ClaimForCreationDto : ClaimDto
    {
        [Required]
        [MaxLength(50)]

        public override string Type { get => base.Type; set => base.Type = value; }

        [Required]
        [MaxLength(50)]
        public override string Value { get => base.Value; set => base.Value = value; }
    }
}