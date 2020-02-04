using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class RoleDto : RoleForCreationDto
    {
        public string Id { get; set; }
        public string NormalizedName { get; set; }
    }
}