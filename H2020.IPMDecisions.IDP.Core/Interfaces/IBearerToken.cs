namespace H2020.IPMDecisions.IDP.Core.Interfaces
{
    public interface IBearerToken
    {
        string Token { get; set; }
        string TokenType { get; }
        string RefreshToken { get; set; }
    }
}