namespace H2020.IPMDecisions.IDP.Core.Entities
{
    public class AuthenticationProviderResult<T>
    {
        public bool IsSuccessful { get; set; }
        public string ResponseMessage { get; set; }
        public T Result { get; set; }
    }
}