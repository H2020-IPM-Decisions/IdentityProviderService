using System;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Models;
using H2020.IPMDecisions.IDP.Data.Core;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.IDP.BLL.ScheduleTasks
{
    public interface IMaintenanceJobs
    {
        void ProcessInactiveUser(IJobCancellationToken token, int weeksInactive, int emailsSent, bool deleteUsers);
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

        public void ProcessInactiveUser(IJobCancellationToken token, int weeksInactive, int emailsSent, bool deleteUsers)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                Task.Run(() => ProcessInactiveUsersOlderThan(weeksInactive, emailsSent, deleteUsers)).Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing schedule to send emails to inactive users. {0}", ex.Message));
            }
        }

        public async Task ProcessInactiveUsersOlderThan(int months, int inactiveEmailsSent = 0, bool deleteUsers = false)
        {
            try
            {
                var users = await this
                    .dataService
                    .UserManagerExtensions
                    .FindAllAsync(u => u.LastValidAccess < DateTime.Now.AddMonths(-months) & u.InactiveEmailsSent == inactiveEmailsSent);

                System.Console.WriteLine(users.Count.ToString());

                foreach (var user in users)
                {
                    if (deleteUsers)
                    {
                        await this.dataService.UserManager.DeleteAsync(user);
                        continue;
                    }

                    var emailToSend = new InactiveUserEmail()
                    {
                        ToAddress = user.Email,
                        InactiveMonths = ((DateTime.Now.Year - user.LastValidAccess.Year) * 12) + DateTime.Now.Month - user.LastValidAccess.Month,
                        AccountDeletionDate = user.LastValidAccess.AddMonths(12).ToShortDateString()
                    };
                    user.InactiveEmailsSent = inactiveEmailsSent + 1;
                }
                // ToDo - Save changes after testing
                //await this.dataService.CompleteAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing schedule to ProcessInactiveUsersOlderThan method. {0}", ex.Message));
            }
        }
    }
}