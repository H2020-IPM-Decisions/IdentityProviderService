using AutoMapper;
using H2020.IPMDecisions.IDP.API.Extensions;
using H2020.IPMDecisions.IDP.Core.Profiles;
using H2020.IPMDecisions.IDP.Core.Services;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Data.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace H2020.IPMDecisions.IDP.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors(Configuration);

            services
                .AddControllers(setupAction =>
                {
                    setupAction.ReturnHttpNotAcceptable = true;
                })
                .AddNewtonsoftJson(setupAction =>
                 {
                     setupAction.SerializerSettings.ContractResolver =
                     new CamelCasePropertyNamesContractResolver();
                 });
                 
            services.ConfigureIdentity();

            services.ConfigureJwtAuthentication(Configuration);

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

            services.AddAutoMapper(typeof(MainProfile));

            services.AddScoped<IDataService, DataService>();

            services.ConfigureMySqlContext(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("IdentityProviderCORS");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
