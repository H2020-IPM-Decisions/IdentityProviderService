using System;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace H2020.IPMDecisions.IDP.API.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequiredClientHeaderAttribute : Attribute, IActionConstraint
    {
        private readonly string[] requestHeaderToMatch;

        public RequiredClientHeaderAttribute(params string[] requestHeaderToMatch)
        {
            this.requestHeaderToMatch = requestHeaderToMatch 
                ?? throw new ArgumentNullException(nameof(requestHeaderToMatch));
        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var requestHeaders = context.RouteContext.HttpContext.Request.Headers;

            foreach (var header in requestHeaderToMatch)
            {
                if (!requestHeaders.ContainsKey(header))
                {
                    return false;
                }
            }
            return true;
        }
    }
}