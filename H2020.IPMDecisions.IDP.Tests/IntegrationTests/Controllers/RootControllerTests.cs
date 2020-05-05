using System.Net;
using System.Net.Http;
using FluentAssertions;
using Xunit;
using Microsoft.AspNetCore.TestHost;

namespace H2020.IPMDecisions.IDP.Tests.IntegrationTests.Controllers
{
    [Collection("FakeWebHost")]
    public class RootControllerTests
    {
        private readonly FakeWebHost fakeWebHost;

        public RootControllerTests(FakeWebHost fakeWebHost)
        {
            this.fakeWebHost = fakeWebHost;
        }

        [Fact]
        public async void Get_NoToken_ShouldReturnOK()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            // Act
            var response = await httpClient.GetAsync("api/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void Post_NoToken_ShouldReturnNotAllowed()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var stringContent = new StringContent("");

            // Act
            var response = await httpClient.PostAsync("api/", stringContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }
    }
}