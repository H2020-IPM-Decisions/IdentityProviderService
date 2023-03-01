using System;

namespace H2020.IPMDecisions.IDP.Core.Models
{
    public class Email
    {
        public string ToAddress { get; set; }
        public Uri CallbackUrl { get; set; }
        public string Token { get; set; }
        public int HoursToConfirmEmail { get; set; } = 24;
        public string Language { get; set; } = "en";

        public void AddLanguage(string language = "en")
        {
            Language = language;
        }
    }    
}