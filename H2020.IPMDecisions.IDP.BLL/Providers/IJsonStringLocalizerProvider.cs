using System;
using H2020.IPMDecisions.IDP.BLL.Helpers;

namespace H2020.IPMDecisions.IDP.BLL.Providers
{
    public interface IJsonStringLocalizerProvider
    {
        IJsonStringLocalizer Create(Type resourceSource);
        IJsonStringLocalizer Create(string baseName, string location);
    }
}