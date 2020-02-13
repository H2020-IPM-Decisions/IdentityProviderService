using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace H2020.IPMDecisions.IDP.API.Filters
{
    internal class AddRequiredClientHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasClientHeaderRequired = context.MethodInfo.GetCustomAttributes(false).OfType<RequiredClientHeaderAttribute>().Any();
            if (!hasClientHeaderRequired) return;

            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "client_id",
                In = ParameterLocation.Header,
                Description = "Client ID",
                Required = true,
                AllowEmptyValue = false,
                Style = ParameterStyle.Simple
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "client_secret",
                In = ParameterLocation.Header,
                Description = "Client Secret",
                Required = true,
                AllowEmptyValue = false,
                Style = ParameterStyle.Simple
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "grant_type",
                In = ParameterLocation.Header,
                Description = "Grant Type",
                Required = true,
                AllowEmptyValue = false,
                Style = ParameterStyle.Simple            
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "refresh_token",
                In = ParameterLocation.Header,
                Description = "Refresh Token",
                Required = false,
                AllowEmptyValue = true,
                Style = ParameterStyle.Simple
            });
        }
    }
}