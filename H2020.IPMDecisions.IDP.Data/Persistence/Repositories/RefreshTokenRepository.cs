using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Helpers;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using H2020.IPMDecisions.IDP.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.IDP.Data.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private ApplicationDbContext context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Create(RefreshToken entity)
        {
            this.context.RefreshToken.Add(entity);
        }

        public void Delete(RefreshToken entity)
        {
            this.context.RefreshToken.Remove(entity);
        }

        public async Task<IEnumerable<RefreshToken>> FindAllAsync()
        {
            return await this.context
                .RefreshToken
                .ToListAsync<RefreshToken>();
        }

        public async Task<PagedList<RefreshToken>> FindAllAsync(RefreshTokenResourceParameter resourceParameter)
        {
             if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));
                
            var collection = this.context.RefreshToken as IQueryable<RefreshToken>;

            return await PagedList<RefreshToken>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<RefreshToken> FindByCondition(Expression<Func<RefreshToken, bool>> expression)
        {
            return await this.context
            .RefreshToken
            .Where(expression)
            .FirstOrDefaultAsync();
        }

        public async Task<RefreshToken> FindByIdAsync(Guid id)
        {
            return await this
                .context
                .RefreshToken
                .SingleOrDefaultAsync(a =>
                    a.Id == id);
        }

        public void Update(RefreshToken entity)
        {
            this.context.RefreshToken.Update(entity);
        }
    }
}