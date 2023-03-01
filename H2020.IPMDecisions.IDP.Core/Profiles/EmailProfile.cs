using H2020.IPMDecisions.IDP.Core.Models;

namespace H2020.IPMDecisions.IDP.Core.Profiles
{
    public class EmailProfile : MainProfile
    {
        public EmailProfile()
        {
            // Models to Models
            CreateMap<Email, RegistrationEmail>();
            CreateMap<Email, ForgotPasswordEmail>();
        }
    }
}