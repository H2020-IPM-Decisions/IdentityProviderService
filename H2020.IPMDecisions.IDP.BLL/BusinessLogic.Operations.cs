using System;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public bool GenerateUserReports()
        {
            try
            {
                RecurringJob.TriggerJob("Run-Accounts-Report");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error generating report. {0}", ex.Message));
                return false;
            }
        }
    }
}