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
        void RemoveInactiveUser(IJobCancellationToken token);
        void SendEmailToInactiveUser(IJobCancellationToken token);
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

        public void SendEmailToInactiveUser(IJobCancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                var monthsInactive = 3;
                Task.Run(() => ProcessInactiveUsersOlderThan(monthsInactive)).Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing schedule to send emails to inactive users. {0}", ex.Message));
            }
        }

        public void RemoveInactiveUser(IJobCancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                // var monthsInactive = 4;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing schedule to remove inactive users. {0}", ex.Message));
            }
        }

        public async Task ProcessInactiveUsersOlderThan(int months)
        {
            try
            {
                var users = await this
                    .dataService
                    .UserManagerExtensions
                    .FindAllAsync(u => u.LastValidAccess < DateTime.Now.AddMonths(-months));

            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}