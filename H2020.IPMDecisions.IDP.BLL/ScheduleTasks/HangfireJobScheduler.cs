using System;
using Hangfire;

namespace H2020.IPMDecisions.IDP.BLL.ScheduleTasks
{
    public class HangfireJobScheduler
    {
        public static void HangfireScheduleJobs()
        {
            RecurringJob.RemoveIfExists(nameof(MaintenanceJobs.SendEmailToInactiveUser));
            RecurringJob.AddOrUpdate<MaintenanceJobs>(nameof(MaintenanceJobs.SendEmailToInactiveUser),
                job => job.SendEmailToInactiveUser(JobCancellationToken.Null),
                Cron.Weekly(DayOfWeek.Monday, 10), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists(nameof(MaintenanceJobs.RemoveInactiveUser));
            RecurringJob.AddOrUpdate<MaintenanceJobs>(nameof(MaintenanceJobs.RemoveInactiveUser),
                job => job.RemoveInactiveUser(JobCancellationToken.Null),
                Cron.Weekly(DayOfWeek.Monday, 9), TimeZoneInfo.Utc);
        }
    }
}