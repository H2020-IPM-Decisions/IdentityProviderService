using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.BLL.Providers;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Core.Models;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Newtonsoft.Json;

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
                logger.LogError(string.Format("Error in BLL - Error executing schedule to send reports. {0}", ex.Message));
            }
        }

        private async Task ProcessTotalAccountsReport()
        {
            try
            {
                var validClaims = this.configuration["AccessClaims:UserAccessLevels"];
                var listOfValidClaims = validClaims.Split(';').ToList();
                var userTypeClaim = this.configuration["AccessClaims:ClaimTypeName"];

                var reportData = await this.internalCommunicationProvider.GetDataFromUPRForReportsAsync();
                var allUsers = new List<ReportUserDataJoined>();

                foreach (var claimValue in listOfValidClaims)
                {
                    var claimAsClaim = new Claim(userTypeClaim.ToLower(), claimValue.ToLower(), ClaimValueTypes.String);
                    var users = await this
                        .dataService
                        .UserManager
                        .GetUsersForClaimAsync(claimAsClaim);

                    if (users == null || users.Count == 0) continue;
                    var result = from user in users
                                 join reportRecord in reportData on user.Id equals reportRecord.UserId into userReportData
                                 from userData in userReportData.DefaultIfEmpty()
                                 select new ReportUserDataJoined()
                                 {
                                     User = this.mapper.Map<ApplicationUserForReport>(user, opt =>
                                        {
                                            opt.Items["userType"] = claimValue.ToLower();
                                        }),
                                     FarmData = userData?.Farm
                                 };
                    allUsers.AddRange(result);
                }

                var reportAsJson = JsonConvert.SerializeObject(allUsers);
                var emailSent = await this.internalCommunicationProvider.SendReportAsync(reportAsJson);
                if (!emailSent)
                    throw new Exception(string.Format("Error sending report! {0}", DateTime.Today.ToString("yyyy_MM_dd")));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing schedule to ProcessTotalAccountsReport method. {0}", ex.Message));
            }
        }
    }
}