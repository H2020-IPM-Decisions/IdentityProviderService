using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Services;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Data.Core.Repositories;
using H2020.IPMDecisions.IDP.Data.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;

namespace H2020.IPMDecisions.IDP.Data.Persistence
{
    public class DataService : IDataService
    {
        private readonly ApplicationDbContext context;
        private readonly IPropertyMappingService propertyMappingService;
        public UserManager<ApplicationUser> UserManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        private IApplicationClientRepository applicationClients;
        public IApplicationClientRepository ApplicationClients
        {
            get
            {
                if (applicationClients == null)
                {
                    applicationClients = new ApplicationClientRepository(this.context, this.propertyMappingService);
                }
                return applicationClients;
            }
        }

        private IUserManagerExtensionRepository userManagerExtensions;
        public IUserManagerExtensionRepository UserManagerExtensions
        {
            get
            {
                if (userManagerExtensions == null)
                {
                    userManagerExtensions = new UserManagerExtensionRepository(this.UserManager, this.propertyMappingService);
                }
                return userManagerExtensions;
            }
        }      

        public DataService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IPropertyMappingService propertyMappingService)
        {
            this.UserManager = userManager 
                ?? throw new ArgumentNullException(nameof(userManager));
            this.RoleManager = roleManager 
                ?? throw new ArgumentNullException(nameof(roleManager));
            this.propertyMappingService = propertyMappingService 
                ?? throw new ArgumentNullException(nameof(propertyMappingService));
            this.context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CompleteAsync()
        {
            if (this.context != null)
                await this.context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (this.context != null)
                this.context.Dispose();
        }
    }
}