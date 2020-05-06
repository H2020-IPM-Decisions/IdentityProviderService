using H2020.IPMDecisions.IDP.Core.Interfaces;

namespace H2020.IPMDecisions.IDP.Core.Models
{
    public class BearerToken : IBearerToken
    {
        public string Token { get; set; }
        public string TokenType { get; private set; } = "Bearer";
        public string RefreshToken { get; set; }
    }
}