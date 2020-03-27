namespace H2020.IPMDecisions.IDP.Core.Models
{
    public class BearerToken
    {
        public string Token { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public string RefreshToken { get; set; }
    }
}