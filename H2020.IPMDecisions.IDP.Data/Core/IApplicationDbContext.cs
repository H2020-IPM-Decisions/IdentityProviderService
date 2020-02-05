using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.IDP.Data.Core
{
    public interface IApplicationDbContext
    {
        DbSet<ApplicationClient> ApplicationClient { get; set; }
    }
}