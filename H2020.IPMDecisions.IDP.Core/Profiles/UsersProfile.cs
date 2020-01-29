using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;

namespace H2020.IPMDecisions.IDP.Core.Profiles
{
    public class UsersProfile : MainProfile
    {
        public UsersProfile()
        {
            // Entities to Dtos
            CreateMap<ApplicationUser, UserDto>();

            // Dtos to Entities
            CreateMap<UserDto, ApplicationUser>();
            CreateMap<UserForRegistrationDto, ApplicationUser>();

        }
    }
}