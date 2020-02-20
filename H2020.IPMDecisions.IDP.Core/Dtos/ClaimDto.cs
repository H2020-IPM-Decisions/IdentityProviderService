using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public abstract class ClaimDto
    {
        public virtual string Type { get; set; }
        public virtual string Value { get; set; }
    }
}