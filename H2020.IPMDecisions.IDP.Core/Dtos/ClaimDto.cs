using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public abstract class ClaimDto
    {
        [MaxLength(50)]
        public virtual string Type { get; set; }
        [MaxLength(50)]
        public virtual string Value { get; set; }
    }
}