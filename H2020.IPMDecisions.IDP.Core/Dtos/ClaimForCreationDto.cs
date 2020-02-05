using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class ClaimForCreationDto : ClaimDto
    {
        [Required]
        public override string Type { get => base.Type; set => base.Type = value; }

        [Required]
        public override string Value { get => base.Value; set => base.Value = value; }
    }
}