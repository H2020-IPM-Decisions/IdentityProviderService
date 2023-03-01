using Microsoft.Extensions.Localization;

namespace H2020.IPMDecisions.IDP.BLL.Helpers
{
    public interface IJsonStringLocalizer
    {
        LocalizedString this[string name] { get; }
        LocalizedString this[string name, params object[] arguments] { get; }
    }
}