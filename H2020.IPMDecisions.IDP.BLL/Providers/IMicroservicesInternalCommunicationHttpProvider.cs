using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Models;

namespace H2020.IPMDecisions.IDP.BLL.Providers
{
    public interface IMicroservicesInternalCommunicationHttpProvider
    {
        Task<bool> SendRegistrationEmail(RegistrationEmail registrationEmail);
        Task<bool> SendForgotPasswordEmail(ForgotPasswordEmail forgotPasswordEmail);
        Task<bool> ResendConfirmationEmail(RegistrationEmail registrationEmail);
        Task<bool> SendInactiveUserEmail(InactiveUserEmail inactiveUserEmail);
        Task<bool> CreateUserProfileAsync(ApplicationUser user);
        bool DeleteUserProfileAsync(Guid userId);
        Task<bool> UserHasDssAsync(Guid userId);
        Task<bool> SendReportAsync(string reportFilePath);
        Task<List<ReportData>> GetDataFromUPRForReportsAsync();
    }
}