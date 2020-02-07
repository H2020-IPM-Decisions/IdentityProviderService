using System.Collections.Generic;

namespace H2020.IPMDecisions.IDP.Core.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
    }
}