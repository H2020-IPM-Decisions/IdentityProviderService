using System;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class RoleDto : RoleForCreationDto
    {
        public Guid Id { get; set; }
        public string NormalizedName { get; set; }
    }
}