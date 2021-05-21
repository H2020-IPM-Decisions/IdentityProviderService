using System;
using AutoMapper;
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
        private readonly IMapper mapper;
        public MaintenanceJobs(
            ILogger<MaintenanceJobs> logger,
            IMapper mapper)
        {
            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public void SendEmailToInactiveUser(IJobCancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                var monthsInactive = 3;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing schedule to send emails to old users. {0}", ex.Message));
            }
        }

        public void RemoveInactiveUser(IJobCancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                var monthsInactive = 4;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing schedule to send emails to old users. {0}", ex.Message));
            }
        }
    }
}