using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.IDP.Data.Persistence.Repositories
{
    public class ApplicationClientRepository : IApplicationClientRepository
    {
        private readonly IApplicationDbContext context;

        public ApplicationClientRepository(IApplicationDbContext context)
        {
            this.context = context;
        }
        
        public void Create(ApplicationClient entity)
        {
            this.context.ApplicationClient.Add(entity);
        }

        public void Delete(ApplicationClient entity)
        {
            this.context.ApplicationClient.Remove(entity);
        }       

        public async Task<List<ApplicationClient>> FindAllAsync()
        {
            return await this.context.ApplicationClient.ToListAsync();
        }

        public async Task<ApplicationClient> FindByIdAsync(Guid id)
        {
            return await this.context
                .ApplicationClient
                .SingleOrDefaultAsync(a =>
                    a.Id == id);
        }

        public async Task<ApplicationClient> FindByNameAsync(string name)
        {
            return await this.context
                .ApplicationClient
                .SingleOrDefaultAsync(a =>
                    a.Name == name);
        }

        public void Update(ApplicationClient entity)
        {
            this.context.ApplicationClient.Update(entity);
        }
    }
}