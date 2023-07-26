using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.API.Filters;
using H2020.IPMDecisions.IDP.BLL.Providers;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Persistence;
using Hangfire;
using Hangfire.MySql.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Extensions.Logging;

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
                        b => b.MigrationsAssembly("H2020.IPMDecisions.IDP.Data"));
                });
        }

        public static void ConfigureIdentity(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(int.Parse(config["IdentityOptions:DefaultLockoutTimeSpan"]));
                options.Lockout.MaxFailedAccessAttempts = int.Parse(config["IdentityOptions:MaxFailedAccessAttempts"]);
                options.Lockout.AllowedForNewUsers = true;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 1;

                // ToDo When Email confirmation available
                // options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.User.RequireUniqueEmail = true;
            });

            services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromHours(int.Parse(config["EmailConfirmationAllowanceHours"])));
        }

        public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var jwtSecretKey = config["JwtSettings:SecretKey"];
            var issuerServerUrl = config["JwtSettings:IssuerServerUrl"];
            var audiencesServersUrl = Audiences(config["JwtSettings:ValidAudiences"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = issuerServerUrl,
                    ValidAudiences = audiencesServersUrl,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "H2020 IPM Decisions - Identity Provider API",
                    Version = "v1",
                    Description = "Identity Provider for the H2020 IPM Decisions project",
                    // TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "ADAS Modelling and Informatics Team",
                        Email = "software@adas.co.uk",
                        Url = new Uri("https://www.adas.uk/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under GNU General Public License v3.0",
                        Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.txt"),
                    }
                });
                c.DescribeAllParametersInCamelCase();

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
                });

                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.OperationFilter<AddRequiredClientHeaderParameter>();

            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        public static void ConfigureCors(this IServiceCollection services, IConfiguration config)
        {
            var allowedHosts = config["AllowedHosts"];
            services.AddCors(options =>
            {
                options.AddPolicy("IdentityProviderCORS", builder =>
                {
                    builder.WithOrigins(allowedHosts);
                });
            });
        }

        public static void ConfigureContentNegotiation(this IServiceCollection services)
        {
            services.AddControllers(setupAction =>
                {
                    setupAction.ReturnHttpNotAcceptable = true;
                })
            .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
                });

            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters
                      .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.h2020ipmdecisions.hateoas+json");
                }
            });
        }

        public static void ConfigureKestrelWebServer(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<KestrelServerOptions>(
                config.GetSection("Kestrel")
            );
        }

        public static void ConfigureHttps(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = int.Parse(config["ASPNETCORE_HTTPS_PORT"]);
            });
        }

        public static void ConfigureLogger(this IServiceCollection services, IConfiguration config)
        {
            LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
        }

        public static void ConfigureInternalCommunicationHttpService(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient<IMicroservicesInternalCommunicationHttpProvider, MicroservicesInternalCommunicationHttpProvider>(client =>
            {
                client.BaseAddress = new Uri(config["MicroserviceInternalCommunication:ApiGatewayAddress"]);
                client.DefaultRequestHeaders.Add(config["MicroserviceInternalCommunication:SecurityTokenCustomHeader"], config["MicroserviceInternalCommunication:SecurityToken"]);
            });
        }

        public static IEnumerable<string> Audiences(string audiences)
        {
            var listOfAudiences = audiences.Split(';').ToList();
            return listOfAudiences;
        }

        public static void ConfigureHangfire(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config["ConnectionStrings:MySqlDbConnection"];

            var options = new MySqlStorageOptions
            {
                PrepareSchemaIfNecessary = true,
                TransactionTimeout = TimeSpan.FromMinutes(30),
                InvisibilityTimeout = TimeSpan.FromMinutes(30)
            };

            services.AddHangfire(configuration =>
            {
                configuration
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseStorage(
                    new MySqlStorage(connectionString, options));
            });

            services.AddHangfireServer();
        }
    }
}