using System.ComponentModel.DataAnnotations;
namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class ResetPasswordDto
    {
    [Required]
    public string UserName { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
    [Required]
    public string Token { get; set; }
    }
}