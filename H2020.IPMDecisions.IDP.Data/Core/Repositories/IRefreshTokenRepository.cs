using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;

namespace H2020.IPMDecisions.IDP.Data.Core.Repositories
{
    public interface IRefreshTokenRepository : IRepositoryBase<RefreshToken, RefreshTokenResourceParameter>
    {
        Task<RefreshToken> FindByCondition(Expression<Func<RefreshToken, bool>> expression);
    }
}