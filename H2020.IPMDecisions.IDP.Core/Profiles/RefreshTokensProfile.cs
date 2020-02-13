using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;

namespace H2020.IPMDecisions.IDP.Core.Profiles
{
    public class RefreshTokensProfile : MainProfile
    {
        public RefreshTokensProfile()
        { 
            // Entities to Dtos
            CreateMap<RefreshToken, RefreshTokenDto>();

            // Dtos to Entities
            CreateMap<RefreshTokenDto, RefreshToken>();
        }
    }
}