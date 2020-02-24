using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public abstract class ClaimDto
    {
        [Required]
        public virtual string Type { get; set; }
        
        [Required]
        public virtual string Value { get; set; }
    }
}