using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Entities
{
    public class ApplicationClient
    {
        [Key]
        [MaxLength(32)]
        public string ClientId { get; set; }

        [Required]
        [MaxLength(80)]
        public string Base64Secret { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public ApplicationClientType ApplicationClientType { get; set; }

        [Required]
        public bool Enabled { get; set; }

        [Required]
        public double RefreshTokenLifeTime { get; set; }
    }

}