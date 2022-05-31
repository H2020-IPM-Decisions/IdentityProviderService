using System;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.BLL.Providers;
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
        private readonly IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider;
        private readonly IMapper mapper;

        public MaintenanceJobs(
            ILogger<MaintenanceJobs> logger,
            IDataService dataService,
            IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider,
            IMapper mapper)
        {
            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
            this.internalCommunicationProvider = internalCommunicationProvider
                ?? throw new ArgumentNullException(nameof(internalCommunicationProvider));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
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
                    .FindAllAsync(u => u.LastValidAccess < DateTime.Now.AddMonths(-months) && u.InactiveEmailsSent == inactiveEmailsSent);

                foreach (var user in users)
                {
                    if (deleteUsers)
                    {
                        await this.dataService.UserManager.DeleteAsync(user);
                        this.internalCommunicationProvider.DeleteUserProfileAsync(Guid.Parse(user.Id));
                        continue;
                    }
                    var emailToSend = this.mapper.Map<InactiveUserEmail>(user);
                    var emailSent = await this.internalCommunicationProvider.SendInactiveUserEmail(emailToSend);
                    if (emailSent)
                        user.InactiveEmailsSent = inactiveEmailsSent + 1;
                }
                await this.dataService.CompleteAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing schedule to ProcessInactiveUsersOlderThan method. {0}", ex.Message));
            }
        }
    }
}