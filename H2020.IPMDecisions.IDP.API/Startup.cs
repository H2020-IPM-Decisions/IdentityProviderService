using AutoMapper;
using H2020.IPMDecisions.IDP.API.Extensions;
using H2020.IPMDecisions.IDP.API.Filters;
using H2020.IPMDecisions.IDP.BLL;
using H2020.IPMDecisions.IDP.BLL.Helpers;
using H2020.IPMDecisions.IDP.BLL.Providers;
using H2020.IPMDecisions.IDP.BLL.ScheduleTasks;
using H2020.IPMDecisions.IDP.Core.Profiles;
using H2020.IPMDecisions.IDP.Core.Services;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Data.Persistence;
using Hangfire;
using Hangfire.Dashboard;
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
        private IWebHostEnvironment CurrentEnvironment { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            CurrentEnvironment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (!CurrentEnvironment.IsDevelopment())
            {
                services.ConfigureHttps(Configuration);
            }

            services.ConfigureKestrelWebServer(Configuration);
            services.ConfigureCors(Configuration);
            services.ConfigureContentNegotiation();

            services.ConfigureIdentity(Configuration);
            services.ConfigureJwtAuthentication(Configuration);
            services.ConfigureInternalCommunicationHttpService(Configuration);

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

            services.AddAutoMapper(typeof(MainProfile));

            services.ConfigureLogger(Configuration);
            services.AddScoped<IDataService, DataService>();
            services.AddSingleton<LocationMiddleware>();
            services.AddTransient<IAuthenticationProvider, AuthenticationProvider>();
            services.AddTransient<IJWTProvider, JWTProvider>();
            services.AddTransient<IRefreshTokenProvider, RefreshTokenProvider>();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IJsonStringLocalizer, JsonStringLocalizer>();
            services.AddSingleton<IJsonStringLocalizerProvider, JsonStringLocalizerProvider>();
            services.AddScoped<IBusinessLogic, BusinessLogic>();
            services.AddScoped<UserAccessingOwnDataActionFilter>();
            services.AddScoped<IsValidUserClaimValueActionFilter>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(serviceProvider =>
            {
                var actionContext = serviceProvider.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = serviceProvider.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
            services.ConfigureHangfire(Configuration);
            services.ConfigureMySqlContext(Configuration);
            services.ConfigureSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime applicationLifetime)
        {
            if (CurrentEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                if (CurrentEnvironment.IsProduction())
                {
                    app.UseForwardedHeaders();
                    app.UseHsts();
                    app.UseHttpsRedirection();
                }
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected error happened. Try again later.");
                    });
                });
            }

            app.UseCors("IdentityProviderCORS");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<LocationMiddleware>();

            var apiBasePath = Configuration["MicroserviceInternalCommunication:IdentityProviderMicroservice"];

            app.UseSwagger(c =>
                        {
                            c.RouteTemplate = apiBasePath + "swagger/{documentName}/swagger.json";
                        });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{apiBasePath}swagger/v1/swagger.json", "H2020 IPM Decisions - Identity Provider API");
                c.RoutePrefix = $"{apiBasePath}swagger";
            });

            var dashboardOptions = new DashboardOptions();
            if (!CurrentEnvironment.IsDevelopment())
            {
                dashboardOptions.Authorization = new[] { new IsAdminFilter() }; ;
                dashboardOptions.IsReadOnlyFunc = (DashboardContext context) => false;
            }
            app.UseHangfireDashboard($"/{apiBasePath}dashboard", dashboardOptions);
            HangfireJobScheduler.HangfireScheduleJobs(Configuration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }
        private void OnShutdown()
        {
            NLog.LogManager.Shutdown();
        }
    }
}
