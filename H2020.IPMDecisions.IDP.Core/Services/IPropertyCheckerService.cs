namespace H2020.IPMDecisions.IDP.Core.Services
{
    public interface IPropertyCheckerService
    {
        bool TypeHasProperties<T>(string fields, bool mustHaveIdField = false);
    }
}