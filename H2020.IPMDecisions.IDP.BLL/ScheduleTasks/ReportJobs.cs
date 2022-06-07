using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.BLL.Providers;
using H2020.IPMDecisions.IDP.Data.Core;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.IDP.BLL.ScheduleTasks
{
    public interface IReportJobs
    {
        void TotalAccountsReport(IJobCancellationToken token);
    }

    public class ReportJobs : IReportJobs
    {
        private readonly ILogger<MaintenanceJobs> logger;
        private readonly IDataService dataService;
        private readonly IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public ReportJobs(
            ILogger<MaintenanceJobs> logger,
            IDataService dataService,
            IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider,
            IMapper mapper,
            IConfiguration configuration)
        {
            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
            this.internalCommunicationProvider = internalCommunicationProvider
                ?? throw new ArgumentNullException(nameof(internalCommunicationProvider));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            this.configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void TotalAccountsReport(IJobCancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                Task.Run(() => ProcessTotalAccountsReport()).Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing schedule to send emails to inactive users. {0}", ex.Message));
            }
        }

        private async Task ProcessTotalAccountsReport()
        {
            try
            {
                var validClaims = this.configuration["AccessClaims:UserAccessLevels"];
                var listOfValidClaims = validClaims.Split(';').ToList();
                var userTypeClaim = this.configuration["AccessClaims:ClaimTypeName"];
                var lastValidAccessDay = int.Parse(this.configuration["Reports:LastValidAccessDays"]);
                var reportEmails = this.configuration.GetSection("Reports:ReportReceiversEmails")?.GetChildren()?.Select(x => x.Value)?.ToList();

                // Get data from UPR, userID, all farms coordinates and DSS selected

                foreach (var claimValue in listOfValidClaims)
                {
                    var claimAsClaim = new Claim(userTypeClaim.ToLower(), claimValue.ToLower(), ClaimValueTypes.String);
                    var users = await this
                        .dataService
                        .UserManager
                        .GetUsersForClaimAsync(claimAsClaim);

                    var userAccessedLast7Days = users
                         .Where(u => u.LastValidAccess > DateTime.Now.AddDays(-lastValidAccessDay));

                    System.Console.WriteLine(claimValue);
                    System.Console.WriteLine(users.Count().ToString());
                    System.Console.WriteLine(userAccessedLast7Days.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing schedule to ProcessTotalAccountsReport method. {0}", ex.Message));
            }
        }
    }
}