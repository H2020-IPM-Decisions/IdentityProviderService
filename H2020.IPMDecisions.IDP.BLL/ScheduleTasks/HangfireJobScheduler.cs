using System;
using Hangfire;

namespace H2020.IPMDecisions.IDP.BLL.ScheduleTasks
{
    public class HangfireJobScheduler
    {
        public static void HangfireScheduleJobs()
        {
            RecurringJob.RemoveIfExists("Send Initial Inactive Email");
            RecurringJob.AddOrUpdate<MaintenanceJobs>("Send Initial Inactive Email",
                job => job.ProcessInactiveUser(JobCancellationToken.Null, 6, 0),
                Cron.Weekly(DayOfWeek.Monday, 12), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists("Send Second Inactive Email");
            RecurringJob.AddOrUpdate<MaintenanceJobs>("Send Second Inactive Email",
                job => job.ProcessInactiveUser(JobCancellationToken.Null, 10, 1),
                Cron.Weekly(DayOfWeek.Monday, 11), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists("Send Last Inactive Email");
            RecurringJob.AddOrUpdate<MaintenanceJobs>("Send Last Inactive Email",
                job => job.ProcessInactiveUser(JobCancellationToken.Null, 11, 2),
                Cron.Weekly(DayOfWeek.Monday, 10), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists("Remove Inactive User");
            RecurringJob.AddOrUpdate<MaintenanceJobs>("Remove Inactive User",
                job => job.ProcessInactiveUser(JobCancellationToken.Null, 12, 3),
                Cron.Weekly(DayOfWeek.Monday, 9), TimeZoneInfo.Utc);
        }
    }
}