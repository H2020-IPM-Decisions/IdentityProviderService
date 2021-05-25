using System;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Data.Core;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.IDP.BLL.ScheduleTasks
{
    public interface IMaintenanceJobs
    {
        void ProcessInactiveUser(IJobCancellationToken token, int weeksInactive, int emailsSent);
    }

    public class MaintenanceJobs : IMaintenanceJobs
    {
        private readonly ILogger<MaintenanceJobs> logger;
        private readonly IDataService dataService;
        public MaintenanceJobs(
            ILogger<MaintenanceJobs> logger,
            IDataService dataService)
        {
            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
        }

        public void ProcessInactiveUser(IJobCancellationToken token, int weeksInactive, int emailsSent)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                Task.Run(() => ProcessInactiveUsersOlderThan(weeksInactive, emailsSent)).Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing schedule to send emails to inactive users. {0}", ex.Message));
            }
        }

        public async Task ProcessInactiveUsersOlderThan(int months, int inactiveEmailsSent = 0)
        {
            try
            {
                var users = await this
                    .dataService
                    .UserManagerExtensions
                    .FindAllAsync(u => u.LastValidAccess < DateTime.Now.AddMonths(-months) & u.InactiveEmailsSent == inactiveEmailsSent);

                foreach (var user in users)
                {
                    user.InactiveEmailsSent = inactiveEmailsSent + 1;
                }

                await this.dataService.CompleteAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}