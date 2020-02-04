using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.IDP.Core.Entities;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public abstract class ApplicationClientForManipulationDto
    {
        [MaxLength(100, ErrorMessage = "Name max length 100 characters")]
        public virtual string Name { get; set; }
        public virtual ApplicationClientType ApplicationClientType { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual int RefreshTokenLifeTime { get; set; }
    }
}