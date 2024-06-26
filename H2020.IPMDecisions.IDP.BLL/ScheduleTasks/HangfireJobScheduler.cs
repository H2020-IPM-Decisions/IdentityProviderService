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
            var emailInactiveUsers = configuration.GetSection(nameof(InactiveUsers)).Get<InactiveUsers>();
            var firstEmail = emailInactiveUsers.FirstEmailMonthInactive;
            var secondEmail = emailInactiveUsers.SecondEmailMonthInactive;
            var lastEmail = emailInactiveUsers.LastEmailMonthInactive;
            var deleteAccount = emailInactiveUsers.DeleteAccountMonthInactive;
            var reportHour = int.Parse(configuration["Reports:HourToSendReport"]);

            RecurringJob.AddOrUpdate<MaintenanceJobs>(string.Format("Send-Initial-Inactive-Email"),
                job => job.ProcessInactiveUser(JobCancellationToken.Null, firstEmail, 0, false),
                Cron.Weekly(DayOfWeek.Wednesday, 10));

            RecurringJob.AddOrUpdate<MaintenanceJobs>(string.Format("Send-Second-Inactive-Email"),
                job => job.ProcessInactiveUser(JobCancellationToken.Null, secondEmail, 1, false),
                Cron.Weekly(DayOfWeek.Tuesday, 10));

            RecurringJob.AddOrUpdate<MaintenanceJobs>(string.Format("Send-Last-Inactive-Email"),
                job => job.ProcessInactiveUser(JobCancellationToken.Null, lastEmail, 2, false),
                Cron.Weekly(DayOfWeek.Monday, 10));

            RecurringJob.AddOrUpdate<MaintenanceJobs>(string.Format("Remove-Inactive-Accounts"),
                job => job.ProcessInactiveUser(JobCancellationToken.Null, deleteAccount, 3, true),
                Cron.Weekly(DayOfWeek.Monday, 9));

            RecurringJob.AddOrUpdate<ReportJobs>("Run-Accounts-Report",
                job => job.TotalAccountsReport(JobCancellationToken.Null),
                Cron.Weekly(DayOfWeek.Sunday, reportHour, 00));
        }
    }
}