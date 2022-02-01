using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace H2020.IPMDecisions.IDP.API.Filters
{
    public class AddLanguageToContextFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var language = context.HttpContext.Request.Headers["Accept-Language"].FirstOrDefault();
                if (string.IsNullOrEmpty(language)) language = "en";

                var culture = new CultureInfo(language);              
                context.HttpContext.Items.Add("language", language);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            catch (Exception ex)
            {
                context.Result = new BadRequestObjectResult(new { message = ex.Message.ToString() }); ;
                return;
            }
        }
    }
}