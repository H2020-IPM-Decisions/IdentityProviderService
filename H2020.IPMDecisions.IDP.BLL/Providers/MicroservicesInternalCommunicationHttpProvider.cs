using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.IDP.BLL.Providers
{
    public class MicroservicesInternalCommunicationHttpProvider : IMicroservicesInternalCommunicationHttpProvider, IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration config;
        private readonly ILogger<MicroservicesInternalCommunicationHttpProvider> logger;

        public MicroservicesInternalCommunicationHttpProvider(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<MicroservicesInternalCommunicationHttpProvider> logger)
        {
            this.httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
            this.config = config ?? throw new System.ArgumentNullException(nameof(config));
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }

        public async Task<bool> SendRegistrationEmail(RegistrationEmail registrationEmail)
        {
            try
            {
                StringContent content = CreateEmailJsonObject(registrationEmail);

                var emailEndPoint = config["MicroserviceInternalCommunication:EmailMicroservice"];
                var emailResponse = await httpClient.PostAsync(emailEndPoint + "internal/registrationemail", content);
                if (!emailResponse.IsSuccessStatusCode)
                {
                    var responseContent = await emailResponse.Content.ReadAsStringAsync();
                    logger.LogWarning(string.Format("Error in Sending RegistrationEmail. Reason: {0}. Response Content: {1}",
                        emailResponse.ReasonPhrase, responseContent));
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in MicroservicesInternalCommunicationHttpProvider - SendRegistrationEmail. {0}", ex.Message));
                return false;
            }
        }

        public async Task<bool> SendForgotPasswordEmail(ForgotPasswordEmail forgotPasswordEmail)
        {
            try
            {
                StringContent content = CreateEmailJsonObject(forgotPasswordEmail);

                var emailEndPoint = config["MicroserviceInternalCommunication:EmailMicroservice"];
                var emailResponse = await httpClient.PostAsync(emailEndPoint + "internal/forgotpassword", content);
                if (!emailResponse.IsSuccessStatusCode)
                {
                    var responseContent = await emailResponse.Content.ReadAsStringAsync();
                    logger.LogWarning(string.Format("Error in Sending ForgotPasswordEmail. Reason: {0}. Response Content: {1}",
                        emailResponse.ReasonPhrase, responseContent));
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in MicroservicesInternalCommunicationHttpProvider - SendForgotPasswordEmail. {0}", ex.Message));
                return false;
            }
        }

        public async Task<bool> ResendConfirmationEmail(RegistrationEmail registrationEmail)
        {
            try
            {
                StringContent content = CreateEmailJsonObject(registrationEmail);

                var emailEndPoint = config["MicroserviceInternalCommunication:EmailMicroservice"];
                var emailResponse = await httpClient.PostAsync(emailEndPoint + "internal/ReConfirmEmail", content);
                if (!emailResponse.IsSuccessStatusCode)
                {
                    var responseContent = await emailResponse.Content.ReadAsStringAsync();
                    logger.LogWarning(string.Format("Error in Sending Re-ConfirmEmail. Reason: {0}. Response Content: {1}",
                        emailResponse.ReasonPhrase, responseContent));
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in MicroservicesInternalCommunicationHttpProvider - Re-ConfirmEmail. {0}", ex.Message));
                return false;
            }
        }

        public async Task<bool> CreateUserProfileAsync(ApplicationUser user)
        {
            try
            {
                var jsonObject = new System.Json.JsonObject();
                jsonObject.Add("userId", user.Id);
                jsonObject.Add("FirstName", new MailAddress(user.Email).User.ToString());
                var customContentType = config["MicroserviceInternalCommunication:ContentTypeHeader"];

                var content = new StringContent(
                    jsonObject.ToString(),
                    Encoding.UTF8,
                    customContentType);

                var userProvisionEndPoint = config["MicroserviceInternalCommunication:UserProvisionMicroservice"];
                var response = await httpClient.PostAsync(userProvisionEndPoint + "internal/userprofile", content);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    logger.LogWarning(string.Format("Error creating UserProfile. Reason: {0}. Response Content: {1}",
                        response.ReasonPhrase, responseContent));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in MicroservicesInternalCommunicationHttpProvider - CreateUserProfileAsync. {0}", ex.Message));
                return false;
            }
        }

        private StringContent CreateEmailJsonObject(Email email)
        {
            try
            {
                var jsonObject = new System.Json.JsonObject();
                jsonObject.Add("toAddress", email.ToAddress);
                jsonObject.Add("callbackUrl", email.CallbackUrl.AbsoluteUri);
                jsonObject.Add("hoursToConfirmEmail", email.HoursToConfirmEmail);
                jsonObject.Add("token", email.Token);
                jsonObject.Add("language", email.Language);
                var customContentType = config["MicroserviceInternalCommunication:ContentTypeHeader"];

                var content = new StringContent(
                    jsonObject.ToString(),
                    Encoding.UTF8,
                    customContentType);
                return content;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in MicroservicesInternalCommunicationHttpProvider - SendForgotPasswordEmail. {0}", ex.Message));
                throw ex;
            }
        }

        public async Task<bool> SendInactiveUserEmail(InactiveUserEmail inactiveUserEmail)
        {
            try
            {
                var jsonObject = new System.Json.JsonObject();
                jsonObject.Add("toAddress", inactiveUserEmail.ToAddress);
                jsonObject.Add("accountDeletionDate", inactiveUserEmail.AccountDeletionDate);
                jsonObject.Add("inactiveMonths", inactiveUserEmail.InactiveMonths);
                var customContentType = config["MicroserviceInternalCommunication:ContentTypeHeader"];

                var content = new StringContent(
                    jsonObject.ToString(),
                    Encoding.UTF8,
                    customContentType);

                var emailEndPoint = config["MicroserviceInternalCommunication:EmailMicroservice"];
                var emailResponse = await httpClient.PostAsync(emailEndPoint + "internal/sendinactiveuser", content);
                if (!emailResponse.IsSuccessStatusCode)
                {
                    var responseContent = emailResponse.Content.ReadAsStringAsync().Result;
                    logger.LogWarning(string.Format("Error creating Sending Inactive User Email. Reason: {0}. Response Content: {1}",
                        emailResponse.ReasonPhrase, responseContent));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in MicroservicesInternalCommunicationHttpProvider - SendInactiveUserEmail. {0}", ex.Message));
                throw ex;
            }
        }

        public bool DeleteUserProfileAsync(Guid userId)
        {
            try
            {
                var customContentType = config["MicroserviceInternalCommunication:ContentTypeHeader"];
                var userProvisionEndPoint = config["MicroserviceInternalCommunication:UserProvisionMicroservice"];
                var content = string.Format(userProvisionEndPoint + "internal/userprofile/{0}", userId);
                var response = httpClient.DeleteAsync(content).Result;

                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    logger.LogWarning(string.Format("Error deleting User profile. Reason: {0}. Response Content: {1}",
                        response.ReasonPhrase, responseContent));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in MicroservicesInternalCommunicationHttpProvider - DeleteUserProfileAsync. {0}", ex.Message));
                return false;
            }
        }

        public async Task<bool> UserHasDssAsync(Guid userId)
        {
            try
            {
                var customContentType = config["MicroserviceInternalCommunication:ContentTypeHeader"];
                var userProvisionEndPoint = config["MicroserviceInternalCommunication:UserProvisionMicroservice"];
                var content = string.Format(userProvisionEndPoint + "internal/hasDss/{0}", userId);
                var response = await httpClient.GetAsync(content);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning(string.Format("Error getting user's DSS. Reason: {0}",
                        response.ReasonPhrase));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in MicroservicesInternalCommunicationHttpProvider - UserHasDssAsync. {0}", ex.Message));
                return false;
            }
        }

        public async Task<bool> SendReportAsync(string reportFilePath)
        {
            try
            {
                var jsonObject = new System.Json.JsonObject();
                var reportEmails = config.GetSection("Reports:ReportReceiversEmails")?.GetChildren()?.Select(x => x.Value)?.ToList();
                var reportEmailsAsString = string.Join(";", reportEmails);
                jsonObject.Add("toAddresses", reportEmailsAsString);

                var customContentType = config["MicroserviceInternalCommunication:ContentTypeHeader"];

                var fileName = Path.GetFileName(reportFilePath);
                using var fileStream = File.OpenRead(reportFilePath);

                var content = new StringContent(
                    jsonObject.ToString(),
                    Encoding.UTF8,
                    customContentType);

                var emailEndPoint = config["MicroserviceInternalCommunication:EmailMicroservice"];
                var emailResponse = await httpClient.PostAsync(emailEndPoint + "internal/sendinternalreport", content);
                if (!emailResponse.IsSuccessStatusCode)
                {
                    var responseContent = emailResponse.Content.ReadAsStringAsync().Result;
                    logger.LogWarning(string.Format("Error creating Sending Internal Report. Reason: {0}. Response Content: {1}",
                        emailResponse.ReasonPhrase, responseContent));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in MicroservicesInternalCommunicationHttpProvider - SendInactiveUserEmail. {0}", ex.Message));
                throw ex;
            }
        }

        public async Task<List<ReportData>> GetDataFromUPRForReportsAsync()
        {
            try
            {
                var customContentType = config["MicroserviceInternalCommunication:ContentTypeHeader"];
                var userProvisionEndPoint = config["MicroserviceInternalCommunication:UserProvisionMicroservice"];
                var content = userProvisionEndPoint + "internal/report";
                var response = await httpClient.GetAsync(content);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning(string.Format("Error getting report data. Reason: {0}",
                        response.ReasonPhrase));
                    return null; ;
                }
                var responseText = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ReportData>>(responseText);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in MicroservicesInternalCommunicationHttpProvider - GetDataFromUPRForReports. {0}", ex.Message));
                return null;
            }
        }
    }
}