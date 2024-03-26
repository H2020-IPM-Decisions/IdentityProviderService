using System;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Models;

namespace H2020.IPMDecisions.IDP.Core.Profiles
{
    public class UsersProfile : MainProfile
    {
        public UsersProfile()
        {
            // Entities to Dtos
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<ApplicationUser, UserWithRolesClaimsDto>();
            CreateMap<ApplicationUser, UserRegistrationReturnDto>();

            // Dtos to Entities
            CreateMap<UserDto, ApplicationUser>();
            CreateMap<UserForRegistrationDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Entities to Models
            CreateMap<ApplicationUser, InactiveUserEmail>()
                .ForMember(dest => dest.ToAddress, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.InactiveMonths, opt => opt.MapFrom(src => (((DateTime.Now.Year - src.LastValidAccess.Year) * 12) + DateTime.Now.Month - src.LastValidAccess.Month)))
                .ForMember(dest => dest.AccountDeletionDate, opt => opt.MapFrom(src => src.LastValidAccess.AddMonths(12).ToString("dd/MM/yyyy")));

            CreateMap<ApplicationUser, ApplicationUserForReport>()
                .ForMember(dest => dest.FirstCharactersUserId, opt => opt.MapFrom(src => src.Id.Substring(0, 6)))
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.RegistrationDate))
                .ForMember(dest => dest.LastValidAccess, opt => opt.MapFrom(src => src.LastValidAccess))
                .AfterMap((src, dest, context) =>
                {
                    dest.UserType = context.Items["userType"].ToString();
                });
        }
    }
}