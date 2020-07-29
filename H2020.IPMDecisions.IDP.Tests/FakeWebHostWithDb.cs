using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.API;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace H2020.IPMDecisions.IDP.Tests
{
    [Trait("Category", "Docker")]
    public class FakeWebHostWithDb : IAsyncLifetime
    {
        public IHost Host;
        public bool IsDatabaseInitialized;
        private ApplicationDbContext _context;

        public readonly Guid DefaultApplicationClientId = Guid.Parse("08d7aa5b-e23c-496e-8946-6d8af6b98dd6");
        public readonly Guid ConfidentialApplicationClientId = Guid.Parse("08d7aa5b-e23c-496e-8946-6d8af6b98dd5");
        public readonly Guid DisableApplicationClientId = Guid.Parse("08d7aa5b-e23c-496e-8946-6d8af6b98dd7");
        public readonly string DefaultApplicationClientSecret = "VdzZzA3lxu-P4krX0n8APfISzujFFKAGn6pbGCd3so8";
        public readonly string DefaultAdminUserEmail = "admin@test.com";
        public readonly string DefaultNormalUserEmail = "user@test.com";
        public readonly string DefaultFailEmailUserEmail = "failemail@test.com";
        public readonly string DefaultNormalUserID = "380f0a69-a009-4c34-8496-9a43c2e069ba";
        public readonly string DefaultUserPassword = "Password1!";

        [Trait("Category", "Docker")]
        public async Task InitializeAsync()
        {
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.Test.json")
              .Build();

            string _databaseName = Guid.NewGuid().ToString();
            var connectionString = $"Server=127.0.0.1,3306;Database=H2020.IPMDecisions.IDP.{_databaseName};Uid=root;Pwd=secret;";

            configuration["ConnectionStrings:MySqlDbConnection"] = connectionString;

            Host = await new HostBuilder()
              .ConfigureWebHost(webBuilder =>
              {
                  webBuilder
                    .UseEnvironment(Environments.Development)
                    .UseStartup<Startup>()
                    .UseTestServer()
                    .UseConfiguration(configuration);
              })
              .StartAsync();

            using (var scope = Host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                _context = services.GetService<ApplicationDbContext>();
                Seed();
            }
        }

        public async Task DisposeAsync()
        {
            using (var scope = Host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                _context = services.GetService<ApplicationDbContext>();
                _context.Database.EnsureDeleted();
            }

            await Host.StopAsync();
            Host.Dispose();
        }

        private void Seed()
        {
            if (!IsDatabaseInitialized)
            {
                using (_context)
                {
                    _context.Database.EnsureDeleted();
                    _context.Database.EnsureCreated();

                    var applicationClientPublic = new ApplicationClient()
                    {
                        Name = "My Test Client",
                        JWTAudienceCategory = "https://testclient.com",
                        Base64Secret = DefaultApplicationClientSecret,
                        Id = DefaultApplicationClientId,
                        ApplicationClientType = ApplicationClientType.Public,
                        Enabled = true,
                        RefreshTokenLifeTime = 90,
                    };

                    _context.ApplicationClient.Add(applicationClientPublic);

                    var applicationClientConfidential = new ApplicationClient()
                    {
                        Name = "My Test Client Confidential",
                        JWTAudienceCategory = "https://testclientconfidential.com",
                        Base64Secret = DefaultApplicationClientSecret,
                        Id = ConfidentialApplicationClientId,
                        ApplicationClientType = ApplicationClientType.Confidential,
                        Enabled = true,
                        RefreshTokenLifeTime = 90,
                    };

                    _context.ApplicationClient.Add(applicationClientConfidential);

                    var applicationClientDisable = new ApplicationClient()
                    {
                        Name = "My Test Client Disable",
                        JWTAudienceCategory = "https://testclientconfidential.com",
                        Base64Secret = DefaultApplicationClientSecret,
                        Id = DisableApplicationClientId,
                        ApplicationClientType = ApplicationClientType.Confidential,
                        Enabled = false,
                        RefreshTokenLifeTime = 90,
                    };

                    _context.ApplicationClient.Add(applicationClientDisable);


                    var adminRole = new IdentityRole()
                    {
                        Id = "dd2f7616-65b7-4456-a6dd-37b25a2c050d",
                        Name = "Admin",
                        NormalizedName = "ADMIN",
                        ConcurrencyStamp = "d4c227a1-bed8-4cbb-b0af-cabec5b6d8a6"
                    };

                    _context.Roles.Add(adminRole);

                    var adminUser = new ApplicationUser()
                    {
                        Id = "380f0a69-a009-4c34-8496-9a43c2e069be",
                        UserName = DefaultAdminUserEmail,
                        NormalizedUserName = DefaultAdminUserEmail.ToUpper(),
                        Email = DefaultAdminUserEmail,
                        NormalizedEmail = DefaultAdminUserEmail.ToUpper(),
                        EmailConfirmed = false,
                        PasswordHash = "AQAAAAEAACcQAAAAEJfYkkq/P/d3+GZjsDeGS4HCjukw0vJNN9fg0mdDBzVbKEdNCHMc8bTtUyo/UGVsSw==",
                        SecurityStamp = "KYK2EHHFUNXK62Z7E7H7BNCAABMUL5PE",
                        ConcurrencyStamp = "963515b9-8e57-4a9c-9286-d86dcf9e5fa0",
                        PhoneNumber = null,
                        PhoneNumberConfirmed = false,
                        LockoutEnd = null,
                        LockoutEnabled = true,
                        AccessFailedCount = 0
                    };
                    _context.Users.Add(adminUser);

                    var normalUser = new ApplicationUser()
                    {
                        Id = "380f0a69-a009-4c34-8496-9a43c2e069ba",
                        UserName = DefaultNormalUserEmail,
                        NormalizedUserName = DefaultNormalUserEmail.ToUpper(),
                        Email = DefaultNormalUserEmail,
                        NormalizedEmail = DefaultNormalUserEmail.ToUpper(),
                        EmailConfirmed = false,
                        PasswordHash = "AQAAAAEAACcQAAAAEJfYkkq/P/d3+GZjsDeGS4HCjukw0vJNN9fg0mdDBzVbKEdNCHMc8bTtUyo/UGVsSw==",
                        SecurityStamp = "KYK2EHHFUNXK62Z7E7H7BNCAABMUL5PE",
                        ConcurrencyStamp = "963515b9-8e57-4a9c-9286-d86dcf9e5fa1",
                        PhoneNumber = null,
                        PhoneNumberConfirmed = false,
                        LockoutEnd = null,
                        LockoutEnabled = true,
                        AccessFailedCount = 0
                    };
                    _context.Users.Add(normalUser);

                    var failEmailUser = new ApplicationUser()
                    {
                        Id = "380f0a69-a009-4c34-8496-9a43c2e069bc",
                        UserName = DefaultFailEmailUserEmail,
                        NormalizedUserName = DefaultFailEmailUserEmail.ToUpper(),
                        Email = DefaultFailEmailUserEmail,
                        NormalizedEmail = DefaultFailEmailUserEmail.ToUpper(),
                        EmailConfirmed = false,
                        PasswordHash = "AQAAAAEAACcQAAAAEJfYkkq/P/d3+GZjsDeGS4HCjukw0vJNN9fg0mdDBzVbKEdNCHMc8bTtUyo/UGVsSw==",
                        SecurityStamp = "KYK2EHHFUNXK62Z7E7H7BNCAABMUL5PE",
                        ConcurrencyStamp = "963515b9-8e57-4a9c-9286-d86dcf9e5fa2",
                        PhoneNumber = null,
                        PhoneNumberConfirmed = false,
                        LockoutEnd = null,
                        LockoutEnabled = true,
                        AccessFailedCount = 0
                    };
                    _context.Users.Add(failEmailUser);

                    var userRoleAdmin = new IdentityUserRole<string>(){
                        UserId = "380f0a69-a009-4c34-8496-9a43c2e069be",
                        RoleId = "dd2f7616-65b7-4456-a6dd-37b25a2c050d"
                    };

                    _context.UserRoles.Add(userRoleAdmin);

                    _context.SaveChanges();
                }
                IsDatabaseInitialized = true;
            }
        }

        [CollectionDefinition("FakeWebHostWithDb")]
        public class FakeWebHostWithDbCollectionFixture : ICollectionFixture<FakeWebHostWithDb>
        {
        }
    }
}