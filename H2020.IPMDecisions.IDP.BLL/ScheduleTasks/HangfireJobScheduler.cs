using System;
using H2020.IPMDecisions.IDP.Core.Configuration;
using Hangfire;
using Microsoft.Extensions.Configuration;

namespace H2020.IPMDecisions.IDP.BLL.ScheduleTasks
{
    public class HangfireJobScheduler
    {
        public static void HangfireScheduleJobs(IConfiguration configuration)
        {
            var emailConfirmationAllowanceHours = configuration.GetSection(nameof(InactiveUsers)).Get<InactiveUsers>();
            var firstEmail = emailConfirmationAllowanceHours.FirstEmailMonthInactive;
            var secondEmail = emailConfirmationAllowanceHours.SecondEmailMonthInactive;
            var lastEmail = emailConfirmationAllowanceHours.LastEmailMonthInactive;
            var deleteAccount = emailConfirmationAllowanceHours.DeleteAccountMonthInactive;

            RecurringJob.AddOrUpdate<MaintenanceJobs>(string.Format("Send Initial Inactive Email After {0} Months", firstEmail),
                job => job.ProcessInactiveUser(JobCancellationToken.Null, firstEmail, 0, false),
                Cron.Weekly(DayOfWeek.Wednesday, 10), TimeZoneInfo.Utc);

            RecurringJob.AddOrUpdate<MaintenanceJobs>(string.Format("Send Second Inactive Email After {0} Months", secondEmail),
                job => job.ProcessInactiveUser(JobCancellationToken.Null, secondEmail, 1, false),
                Cron.Weekly(DayOfWeek.Tuesday, 10), TimeZoneInfo.Utc);

            RecurringJob.AddOrUpdate<MaintenanceJobs>(string.Format("Send Last Inactive Email After {0} Months", lastEmail),
                job => job.ProcessInactiveUser(JobCancellationToken.Null, lastEmail, 2, false),
                Cron.Weekly(DayOfWeek.Monday, 10), TimeZoneInfo.Utc);

            RecurringJob.AddOrUpdate<MaintenanceJobs>(string.Format("Remove Inactive User After {0} Months", deleteAccount),
                job => job.ProcessInactiveUser(JobCancellationToken.Null, deleteAccount, 3, true),
                Cron.Weekly(DayOfWeek.Monday, 9), TimeZoneInfo.Utc);
        }
    }
}