using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
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

        public async Task<IEnumerable<ApplicationClient>> FindAllAsync()
        {
            return await this.context
                .ApplicationClient
                .ToListAsync<ApplicationClient>();
        }

        public async Task<IEnumerable<ApplicationClient>> FindAllAsync(BaseResourceParameter resourceParameter)
        {
            if (string.IsNullOrEmpty(resourceParameter.SearchQuery))
            {
                return await FindAllAsync();
            }

            var collection = this.context.ApplicationClient as IQueryable<ApplicationClient>;

            var searchQuery = resourceParameter.SearchQuery.Trim();

            collection = collection.Where(ac => 
                ac.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                || ac.Url.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));

            return await collection.ToListAsync<ApplicationClient>();
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