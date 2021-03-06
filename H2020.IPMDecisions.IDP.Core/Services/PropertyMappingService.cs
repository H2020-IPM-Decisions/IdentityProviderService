using System;
using System.Collections.Generic;
using System.Linq;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;

namespace H2020.IPMDecisions.IDP.Core.Services
{

    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _applicationClientMappingService =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", new PropertyMappingValue(new List<string>() { "Name" })},
                { "JWTAudienceCategory", new PropertyMappingValue(new List<string>() { "JWTAudienceCategory" })}
            };

        private Dictionary<string, PropertyMappingValue> _applicationUserMappingService =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
                { "Email", new PropertyMappingValue(new List<string>() { "Email" })}
           };


        public IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            this.propertyMappings.Add(new PropertyMapping<ApplicationClientDto, ApplicationClient>(_applicationClientMappingService));
            this.propertyMappings.Add(new PropertyMapping<UserDto, ApplicationUser>(_applicationUserMappingService));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = this.propertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
                return matchingMapping.First()._mappingDictionary;

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
                return true;

            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                    return false;
            }
            return true;
        }
    }
}