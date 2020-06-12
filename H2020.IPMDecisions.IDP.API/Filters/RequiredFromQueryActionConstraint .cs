using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace H2020.IPMDecisions.IDP.API.Filters
{
    public class RequiredFromQueryActionConstraint : IActionConstraint
    {
        private readonly string parameter;
        public RequiredFromQueryActionConstraint(string parameter)
        {
            this.parameter = parameter;
        }

        public int Order => 999;

        public bool Accept(ActionConstraintContext context)
        {
            if (!context.RouteContext.HttpContext.Request.Query.ContainsKey(parameter))
            {
                return false;
            }

            return true;
        }
    }
}