using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class UserForPartialUpdateDto
    {
        public bool EmailConfirmed { get; set; } = true;

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}