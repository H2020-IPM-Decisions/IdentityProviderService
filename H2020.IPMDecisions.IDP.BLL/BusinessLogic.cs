using System;
using AutoMapper;
using H2020.IPMDecisions.IDP.BLL.Providers;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        private readonly IMapper mapper;
        private readonly IDataService dataService;
        private readonly IAuthenticationProvider authenticationProvider;
        private readonly IJWTProvider jWTProvider;
        private readonly IRefreshTokenProvider refreshTokenProvider;
        private readonly IPropertyCheckerService propertyCheckerService;
        private readonly IUrlHelper url;
        private readonly IConfiguration configuration;
        private readonly IPropertyMappingService propertyMappingService;
        private static readonly NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        public BusinessLogic(
            IMapper mapper,
            IDataService dataService,
            IAuthenticationProvider authenticationProvider,
            IJWTProvider jWTProvider,
            IRefreshTokenProvider refreshTokenProvider,
            IPropertyCheckerService propertyCheckerService,
            IPropertyMappingService propertyMappingService,
            IUrlHelper url,
            IConfiguration configuration)
        {
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
            this.authenticationProvider = authenticationProvider
                ?? throw new ArgumentNullException(nameof(authenticationProvider));
            this.jWTProvider = jWTProvider
                ?? throw new ArgumentNullException(nameof(jWTProvider));
            this.refreshTokenProvider = refreshTokenProvider
                ?? throw new ArgumentNullException(nameof(refreshTokenProvider));
            this.propertyCheckerService = propertyCheckerService
               ?? throw new ArgumentNullException(nameof(propertyCheckerService));
            this.propertyMappingService = propertyMappingService
                ?? throw new ArgumentNullException(nameof(propertyMappingService));
            this.url = url 
                ?? throw new ArgumentNullException(nameof(url));
            this.configuration = configuration 
                ?? throw new ArgumentNullException(nameof(configuration));

            //Dummy logger output to assist initial testing of NLog
            //logger.Debug("BusinessLogic Initialising");
            //logger.Info("BusinessLogic Initialising Information");
            //logger.Error("BusinessLogic Initialising Error Example");
        }        
    }
}
