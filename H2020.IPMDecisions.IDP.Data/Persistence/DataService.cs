using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Data.Core.Repositories;
using H2020.IPMDecisions.IDP.Data.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;

namespace H2020.IPMDecisions.IDP.Data.Persistence
{
    public class DataService : IDataService
    {
        public ApplicationDbContext Context { get; set; }
        private IApplicationClientRepository applicationClients;
        public IApplicationClientRepository ApplicationClients
        {
            get
            {
                if (applicationClients == null)
                {
                    applicationClients = new ApplicationClientRepository(this.Context);
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
                    userManagerExtensions = new UserManagerExtensionRepository(this.UserManager);
                }
                return userManagerExtensions;
            }
        }
        
        public UserManager<ApplicationUser> UserManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }       

        public DataService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.UserManager = userManager 
                ?? throw new ArgumentNullException(nameof(userManager));
            this.RoleManager = roleManager 
                ?? throw new ArgumentNullException(nameof(roleManager));
            this.Context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CompleteAsync()
        {
            if (this.Context != null)
                await this.Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (this.Context != null)
                this.Context.Dispose();
        }
    }
}