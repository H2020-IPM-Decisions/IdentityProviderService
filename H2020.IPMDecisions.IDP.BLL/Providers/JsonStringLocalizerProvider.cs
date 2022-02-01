using System;
using H2020.IPMDecisions.IDP.BLL.Helpers;
using Microsoft.Extensions.Caching.Distributed;

namespace H2020.IPMDecisions.IDP.BLL.Providers
{
    public class JsonStringLocalizerProvider : IJsonStringLocalizerProvider
    {
        private readonly IDistributedCache distributedCache;

        public JsonStringLocalizerProvider(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public IJsonStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(distributedCache);
        }

        public IJsonStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer(distributedCache);
        }
    }
}