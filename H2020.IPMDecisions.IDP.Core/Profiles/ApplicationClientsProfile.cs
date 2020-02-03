using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;

namespace H2020.IPMDecisions.IDP.Core.Profiles
{
    public class ApplicationClientsProfile : MainProfile
    {
        public ApplicationClientsProfile()
        {
            // Entities to Dtos
            CreateMap<ApplicationClient, ApplicationClientDto>();

            // Dtos to Entities
            CreateMap<ApplicationClientDto, ApplicationClient>();
            CreateMap<ApplicationClientForCreationDto, ApplicationClient>();
        }
    }
}