using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace H2020.IPMDecisions.IDP.API.Helpers
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(
               this TSource source)
        {
            if (source is null)
            {
                throw new System.ArgumentNullException(nameof(source));
            }

            var dataShapedObject = new ExpandoObject();
            var propertyInfos = typeof(TSource)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var propertyInfo in propertyInfos)
            {
                var propertyValue = propertyInfo.GetValue(source);
                ((IDictionary<string, object>)dataShapedObject)
                    .Add(propertyInfo.Name, propertyValue);
            }
            
            return dataShapedObject;
        }
    }
}