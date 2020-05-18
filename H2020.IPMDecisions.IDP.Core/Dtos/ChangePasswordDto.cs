using System;

using System.ComponentModel.DataAnnotations;
namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class ChangePasswordDto
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
    }
}