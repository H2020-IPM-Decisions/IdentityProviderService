using System;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class RefreshTokenDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ApplicationClientId { get; set; }
        public string ProtectedTicket { get; set; }
        public DateTime ExpiresUtc { get; set; }
    }
}