using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.API;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Persistence;
using Microsoft.AspNetCore.Hosting;
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
        private static bool _databaseInitialized;
        private string _connectionString;
        private ApplicationDbContext _context;

        [Trait("Category", "Docker")]
        public async Task InitializeAsync()
        {
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.Test.json")
              .Build();

            string _databaseName = Guid.NewGuid().ToString();
            _connectionString = $"Server=127.0.0.1,3306;Database=H2020.IPMDecisions.IDP.{_databaseName};Uid=root;Pwd=secret;";
            
            configuration["ConnectionStrings:MySqlDbConnection"] = _connectionString;       

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
            if (!_databaseInitialized)
            {
                using (_context)
                {
                    _context.Database.EnsureDeleted();
                    _context.Database.EnsureCreated();

                    var applicationClient = new ApplicationClient()
                    { 
                        Name = "My Test Client", 
                        Url = "https://testclient.com",
                        Base64Secret = "VdzZzA3lxu-P4krX0n8APfISzujFFKAGn6pbGCd3so8",
                        Id = Guid.Parse("08d7aa5b-e23c-496e-8946-6d8af6b98dd6"),
                        ApplicationClientType = 0,
                        Enabled = true,
                        RefreshTokenLifeTime = 90,
                    };

                    _context.ApplicationClient.Add(applicationClient);

                    _context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }

        [CollectionDefinition("FakeWebHostWithDb")]
        public class FakeWebHostWithDbCollectionFixture : ICollectionFixture<FakeWebHostWithDb>
        {
        }
    }
}