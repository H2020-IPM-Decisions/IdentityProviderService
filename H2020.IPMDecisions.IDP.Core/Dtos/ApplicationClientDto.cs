using System;

namespace H2020.IPMDecisions.IDP.Core.Dtos
{
    public class ApplicationClientDto : ApplicationClientForCreationDto
    {
        public Guid Id { get; set; }
        public string Base64Secret { get; set; }
    }
}