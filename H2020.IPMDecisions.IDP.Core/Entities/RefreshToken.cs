using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.IDP.Core.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ApplicationClientId { get; set; }

        [Required]
        public string ProtectedTicket { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationClient ApplicationClient { get; set; }
    }
}