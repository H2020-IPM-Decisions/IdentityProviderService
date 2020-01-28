using H2020.IPMDecisions.IDP.Data.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace H2020.IPMDecisions.IDP.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config["ConnectionStrings:MySqlDbConnection"];
            services
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseMySql(connectionString, 
                        b => b.MigrationsAssembly("H2020.IPMDecisions.IDP.API"));
                });
        }
    }
}