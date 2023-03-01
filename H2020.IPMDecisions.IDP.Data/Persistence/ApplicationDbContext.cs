using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Data.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.IDP.Data.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public DbSet<ApplicationClient> ApplicationClient { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.Entity<IdentityRole>().ToTable("Role");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");
        }
    }
}