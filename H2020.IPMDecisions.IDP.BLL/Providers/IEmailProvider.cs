using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Models;

namespace H2020.IPMDecisions.IDP.BLL.Providers
{
    public interface IEmailProvider
    {
        Task<bool> SendRegistrationEmail(RegistrationEmail registrationEmail);   
        Task<bool> SendForgotPasswordEmail(ForgotPasswordEmail forgotPasswordEmail);
        Task<bool> ResendConfirmationEmail(RegistrationEmail registrationEmail);
    }
}