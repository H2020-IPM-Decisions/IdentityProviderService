using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Helpers;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;

namespace H2020.IPMDecisions.IDP.Data.Core.Repositories
{
    public interface IUserManagerExtensionRepository
    {
        Task<PagedList<ApplicationUser>> FindAllAsync(ApplicationUserResourceParameter resourceParameter);
        Task<List<ApplicationUser>> FindAllAsync(Expression<Func<ApplicationUser, bool>> expression);
    }
}