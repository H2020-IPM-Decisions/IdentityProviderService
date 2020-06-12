using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace H2020.IPMDecisions.IDP.Tests
{
    public class FakeWebHost : IAsyncLifetime
    {
        public IHost Host;
        public async Task InitializeAsync()
        {
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.Test.json")
              .Build();

            Host = await new HostBuilder()
              .ConfigureWebHost(webBuilder =>
              {
                  webBuilder
                   .UseEnvironment(Microsoft.Extensions.Hosting.Environments.Staging)
                   .UseStartup<Startup>()
                   .UseTestServer()
                   .UseConfiguration(configuration);
              })
              .StartAsync();
        }

        public async Task DisposeAsync()
        {
            await Host?.StopAsync();
            Host?.Dispose();
        }

        [CollectionDefinition("FakeWebHost")]
        public class FakeWebHostCollectionFixture : ICollectionFixture<FakeWebHost>
        {
        }
    }
}