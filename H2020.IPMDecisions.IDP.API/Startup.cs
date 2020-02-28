using AutoMapper;
using H2020.IPMDecisions.IDP.API.Extensions;
using H2020.IPMDecisions.IDP.BLL;
using H2020.IPMDecisions.IDP.BLL.Providers;
using H2020.IPMDecisions.IDP.Core.Profiles;
using H2020.IPMDecisions.IDP.Core.Services;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Data.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            services.ConfigureContentNegotiation();

            services.ConfigureIdentity();
            services.ConfigureJwtAuthentication(Configuration);

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

            services.AddAutoMapper(typeof(MainProfile));

            services.AddScoped<IDataService, DataService>();
            services.AddTransient<IAuthenticationProvider, AuthenticationProvider>();
            services.AddTransient<IJWTProvider, JWTProvider>();
            services.AddTransient<IRefreshTokenProvider, RefreshTokenProvider>();
            services.AddScoped<IBusinessLogic, BusinessLogic>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            services.ConfigureMySqlContext(Configuration);

            services.ConfigureSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // app.UseHsts();
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected error happened. Try again later.");
                    });
                });
            }
            app.UseHsts();

            app.UseCors("IdentityProviderCORS");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "H2020 IPM Decisions - Identity Provider API");
            });
        }
    }
}
