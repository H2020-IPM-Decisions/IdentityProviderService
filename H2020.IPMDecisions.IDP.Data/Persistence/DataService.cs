using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Data.Core.Repositories;
using H2020.IPMDecisions.IDP.Data.Persistence.Repositories;

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
        public DataService(ApplicationDbContext context)
        {
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