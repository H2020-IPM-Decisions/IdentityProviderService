using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.Extensions.Configuration;

namespace H2020.IPMDecisions.IDP.BLL.Providers
{
    public class EmailProvider : IEmailProvider
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration config;

        public EmailProvider(
            HttpClient httpClient,
            IConfiguration config)
        {
            this.config = config 
                ?? throw new ArgumentNullException(nameof(config));
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
                        // ToDo Log didn't sent email
                        var responseContent = await emailResponse.Content.ReadAsStringAsync();
                        //log emailResponse.ReasonPhrase & responseContent 
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                //TODO: log error
                System.Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> SendForgotPasswordEmail(ForgotPasswordEmail forgotPasswordEmail)
        {
            using (httpClient)
            {
                StringContent content = CreateEmailJsonObject(forgotPasswordEmail);

                var emailResponse = await httpClient.PostAsync("accounts/ForgotPassword", content);

                if (!emailResponse.IsSuccessStatusCode)
                {
                    // ToDo Log didn't sent email
                    var responseContent = await emailResponse.Content.ReadAsStringAsync();
                    //log emailResponse.ReasonPhrase & responseContent 
                    return false;
                }
                return true;
            }
        }

        private static StringContent CreateEmailJsonObject(Email email)
        {
            var jsonObject = new System.Json.JsonObject();
            jsonObject.Add("toAddress", email.ToAddress);
            jsonObject.Add("callbackUrl", email.CallbackUrl.AbsoluteUri);
            jsonObject.Add("token", email.Token);          
              
            //config["IPMEmailMicroservice:ContentTypeHeader"];
            var customContentType = "application/vnd.h2020ipmdecisions.email+json"; 

            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                customContentType);
            return content;
        }
    }
}