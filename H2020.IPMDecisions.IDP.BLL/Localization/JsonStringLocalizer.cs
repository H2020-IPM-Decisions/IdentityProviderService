using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace H2020.IPMDecisions.IDP.BLL.Localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        public LocalizedString this[string name] => throw new System.NotImplementedException();

        public LocalizedString this[string name, params object[] arguments] => throw new System.NotImplementedException();

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new System.NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}