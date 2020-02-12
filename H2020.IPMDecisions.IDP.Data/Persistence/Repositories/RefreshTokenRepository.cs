using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Helpers;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using H2020.IPMDecisions.IDP.Data.Core.Repositories;

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
            throw new NotImplementedException();
        }

        public void Delete(RefreshToken entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RefreshToken>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<RefreshToken>> FindAllAsync(RefreshTokenResourceParameter resourceParameter)
        {
            throw new NotImplementedException();
        }

        public Task<RefreshToken> FindByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(RefreshToken entity)
        {
            throw new NotImplementedException();
        }
    }
}