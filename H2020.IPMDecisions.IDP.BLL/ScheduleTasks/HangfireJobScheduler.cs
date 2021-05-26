using System;
using Hangfire;

namespace H2020.IPMDecisions.IDP.BLL.ScheduleTasks
{
    public class HangfireJobScheduler
    {
        public static void HangfireScheduleJobs()
        {
            RecurringJob.RemoveIfExists("Send Initial Inactive Email After 6 Months");
            RecurringJob.AddOrUpdate<MaintenanceJobs>("Send Initial Inactive Email After 6 Months",
                job => job.ProcessInactiveUser(JobCancellationToken.Null, 6, 0, false),
                Cron.Weekly(DayOfWeek.Monday, 12), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists("Send Second Inactive Email After 10 Months");
            RecurringJob.AddOrUpdate<MaintenanceJobs>("Send Second Inactive Email After 10 Months",
                job => job.ProcessInactiveUser(JobCancellationToken.Null, 10, 1, false),
                Cron.Weekly(DayOfWeek.Monday, 11), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists("Send Last Inactive Email After 11 Months");
            RecurringJob.AddOrUpdate<MaintenanceJobs>("Send Last Inactive Email After 11 Months",
                job => job.ProcessInactiveUser(JobCancellationToken.Null, 11, 2, false),
                Cron.Weekly(DayOfWeek.Monday, 10), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists("Remove Inactive User After 12 Months");
            RecurringJob.AddOrUpdate<MaintenanceJobs>("Remove Inactive User After 12 Months",
                job => job.ProcessInactiveUser(JobCancellationToken.Null, 12, 3, true),
                Cron.Weekly(DayOfWeek.Monday, 9), TimeZoneInfo.Utc);
        }
    }
}