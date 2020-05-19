using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Models;

namespace H2020.IPMDecisions.IDP.BLL.Providers
{
    public class EmailProvider : IEmailProvider
    {
        private readonly HttpClient httpClient;

        public EmailProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient
                ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        public async Task<bool> SendRegistrationEmail(RegistrationEmail registrationEmail)
        {
            try
            {
                using (httpClient)
                {
                    var jsonObject = new System.Json.JsonObject();
                    jsonObject.Add("toAddress", registrationEmail.ToAddress);
                    jsonObject.Add("confirmEmailUrl", registrationEmail.ConfirmEmailUrl);
                    var content = new StringContent(
                        jsonObject.ToString(),
                        Encoding.UTF8,
                        "application/vnd.h2020ipmdecisions.email+json");

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
    }
}