using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.IDP.BLL.Providers
{
    public class EmailProvider : IEmailProvider
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration config;
        private readonly ILogger<EmailProvider> logger;

        public EmailProvider(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<EmailProvider> logger)
        {
            this.config = config 
                ?? throw new ArgumentNullException(nameof(config));
            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this.httpClient = httpClient
                ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        public async Task<bool> SendRegistrationEmail(RegistrationEmail registrationEmail)
        {
            try
            {
                using (httpClient)
                {
                    StringContent content = CreateEmailJsonObject(registrationEmail);

                    var emailResponse = await httpClient.PostAsync("accounts/registrationemail", content);

                    if (!emailResponse.IsSuccessStatusCode)
                    {
                        var responseContent = await emailResponse.Content.ReadAsStringAsync();
                        logger.LogWarning(string.Format("Error in Sending RegistrationEmail. Reason: {0}. Response Content: {1}",
                            emailResponse.ReasonPhrase, responseContent));
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in EmailProvider - SendRegistrationEmail. {0}", ex.Message));
                return false;
            }
        }

        public async Task<bool> SendForgotPasswordEmail(ForgotPasswordEmail forgotPasswordEmail)
        {
            try
            {
                using (httpClient)
                {
                    StringContent content = CreateEmailJsonObject(forgotPasswordEmail);

                    var emailResponse = await httpClient.PostAsync("accounts/forgotpassword", content);

                    if (!emailResponse.IsSuccessStatusCode)
                    {
                        var responseContent = await emailResponse.Content.ReadAsStringAsync();
                        logger.LogWarning(string.Format("Error in Sending ForgotPasswordEmail. Reason: {0}. Response Content: {1}",
                            emailResponse.ReasonPhrase, responseContent));
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in EmailProvider - SendForgotPasswordEmail. {0}", ex.Message));
                return false;
            }           
        }

        public async Task<bool> ResendConfirmationEmail(RegistrationEmail registrationEmail)
        {
            try
            {
                using (httpClient)
                {
                    StringContent content = CreateEmailJsonObject(registrationEmail);

                    var emailResponse = await httpClient.PostAsync("accounts/ReConfirmEmail", content);

                    if (!emailResponse.IsSuccessStatusCode)
                    {
                        var responseContent = await emailResponse.Content.ReadAsStringAsync();
                        logger.LogWarning(string.Format("Error in Sending Re-ConfirmEmail. Reason: {0}. Response Content: {1}",
                            emailResponse.ReasonPhrase, responseContent));
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in EmailProvider - Re-ConfirmEmail. {0}", ex.Message));
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
                jsonObject.Add("token", email.Token);
                var customContentType = config["IPMEmailMicroservice:ContentTypeHeader"];

                var content = new StringContent(
                    jsonObject.ToString(),
                    Encoding.UTF8,
                    customContentType);
                return content;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in EmailProvider - SendForgotPasswordEmail. {0}", ex.Message));
                throw ex;
            }            
        }
    }
}