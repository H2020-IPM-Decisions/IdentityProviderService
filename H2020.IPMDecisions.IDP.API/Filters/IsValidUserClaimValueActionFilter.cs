using System;
using System.Linq;
using H2020.IPMDecisions.IDP.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace H2020.IPMDecisions.IDP.API.Filters
{
    public class IsValidUserClaimValueActionFilter : IActionFilter
    {
        private readonly IConfiguration configuration;

        public IsValidUserClaimValueActionFilter(IConfiguration configuration)
        {
            this.configuration = configuration 
                ?? throw new System.ArgumentNullException(nameof(configuration));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var data = (UserForRegistrationDto)context.ActionArguments["userForRegistration"];            
            var userType = data.UserType.ToString();

            var validClaims = this.configuration["AccessClaims:ValidUserAccessClaims"];
            var listOfValidClaims = validClaims.Split(';').ToList();

            if (!listOfValidClaims.Any(str => str.Contains(userType, StringComparison.OrdinalIgnoreCase)))
            {
                context.Result = new BadRequestObjectResult($"'{nameof(userType)}' should be one of the following values: '{validClaims}'");
                return;
            }
        }
    }
}