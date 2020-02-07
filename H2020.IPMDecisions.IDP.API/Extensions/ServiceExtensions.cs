using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // ToDo When Email confirmation available
                // options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.User.RequireUniqueEmail = true;
            });
        }

        public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var jwtSecretKey = config["JwtSettings:SecretKey"];
            var issuerServerUrl = config["JwtSettings:IssuerServerUrl"];
            var audiencesServersUrl = Audiences(config["JwtSettings:ValidAudiencesUrls"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = issuerServerUrl,
                    ValidAudiences = audiencesServersUrl,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
                };
            });
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

        public static IEnumerable<string> Audiences(string audiences)
        {
            var listOfAudiences = audiences.Split(';').ToList();
            return listOfAudiences;
        }
    }
}