using System;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class RoleDto : RoleForManipulationDto
    {
        public Guid Id { get; set; }
        public string NormalizedName { get; set; }
    }
}