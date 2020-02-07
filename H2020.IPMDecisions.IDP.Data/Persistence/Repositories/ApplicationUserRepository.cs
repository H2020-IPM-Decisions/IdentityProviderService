using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Helpers;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using H2020.IPMDecisions.IDP.Data.Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.IDP.Data.Persistence.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public ApplicationUserRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.context = context;
        }

        public void Create(ApplicationUser entity)
        {
            // Not implemented
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser entity, string password)
        {
            return await this.userManager.CreateAsync(entity, password);
        }

        public void Delete(ApplicationUser entity)
        {
            // Not implemented
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser entity)
        {
            return await this.userManager.DeleteAsync(entity);
        }

        public async Task<IEnumerable<ApplicationUser>> FindAllAsync()
        {
            return await this.userManager.Users.ToListAsync();
        }

        public async Task<PagedList<ApplicationUser>> FindAllAsync(ApplicationUserResourceParameter resourceParameter)
        {
            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));
            var collection = this.userManager.Users as IQueryable<ApplicationUser>;

            if (!string.IsNullOrEmpty(resourceParameter.SearchQuery))
            {
                var searchQuery = resourceParameter.SearchQuery.Trim();
                collection = collection.Where(u =>
                    u.UserName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                    || u.Email.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            return await PagedList<ApplicationUser>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<ApplicationUser> FindByIdAsync(Guid id)
        {
            return await this.userManager.FindByIdAsync(id.ToString());
        }

        public async Task<ApplicationUser> FindByNameAsync(string name)
        {
            return await this.userManager.FindByNameAsync(name);
        }

        public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser entity)
        {
            return await this.userManager.GetClaimsAsync(entity);
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser entity)
        {
            return await this.userManager.GetRolesAsync(entity);
        }

        public void Update(ApplicationUser entity)
        {
            // Not implemented
        }
    }
}