
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace H2020.IPMDecisions.IDP.Tests.IntegrationTests.Controllers
{
    [Trait("Category", "Docker")]
    public class AccountsControllerNoDbTests : IClassFixture<FakeWebHost>
    {
        private FakeWebHost fakeWebHost;
        public AccountsControllerNoDbTests(FakeWebHost fakeWebHost)
        {
            this.fakeWebHost = fakeWebHost;
        }

        [Fact]
        public async void PostAuthenticate_WrongGrantTypeHeader_BadRequest()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient
              .DefaultRequestHeaders
              .Add("client_id", "123");
            httpClient
              .DefaultRequestHeaders
              .Add("client_secret", "123");
            httpClient
              .DefaultRequestHeaders
              .Add("grant_type", "Bad");

            var jsonObject = new System.Json.JsonObject();
            const string userEmail = "newuser@test.com";
            jsonObject.Add("email", userEmail);
            jsonObject.Add("password", "Password1!");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("/api/accounts/authenticate", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("Wrong grant type");
        }

        [Fact]
        public async void PostAuthenticate_WrongFormatClientIdHeader_BadRequest()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient
              .DefaultRequestHeaders
              .Add("client_id", "123");
            httpClient
              .DefaultRequestHeaders
              .Add("client_secret", "123");
            httpClient
              .DefaultRequestHeaders
              .Add("grant_type", "password");

            var jsonObject = new System.Json.JsonObject();
            const string userEmail = "newuser@test.com";
            jsonObject.Add("email", userEmail);
            jsonObject.Add("password", "Password1!");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("/api/accounts/authenticate", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("Invalid client Id");
        }

        [Fact]
        public async void PostAuthenticate_MissingClientIdHeader_NotFound()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient
              .DefaultRequestHeaders
              .Add("client_id", "");
            httpClient
              .DefaultRequestHeaders
              .Add("client_secret", "123");
            httpClient
              .DefaultRequestHeaders
              .Add("grant_type", "password");

            var jsonObject = new System.Json.JsonObject();
            const string userEmail = "newuser@test.com";
            jsonObject.Add("email", userEmail);
            jsonObject.Add("password", "Password1!");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("/api/accounts/authenticate", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void PostAuthenticate_MissingSecretIdHeader_NotFound()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient
              .DefaultRequestHeaders
              .Add("client_id", "123");
            httpClient
              .DefaultRequestHeaders
              .Add("client_secret", "");
            httpClient
              .DefaultRequestHeaders
              .Add("grant_type", "password");

            var jsonObject = new System.Json.JsonObject();
            const string userEmail = "newuser@test.com";
            jsonObject.Add("email", userEmail);
            jsonObject.Add("password", "Password1!");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("/api/accounts/authenticate", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void PostAuthenticate_MissingUserEmail_BadRequest()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient
              .DefaultRequestHeaders
              .Add("client_id", "123");
            httpClient
              .DefaultRequestHeaders
              .Add("client_secret", "123");
            httpClient
              .DefaultRequestHeaders
              .Add("grant_type", "password");

            var jsonObject = new System.Json.JsonObject();
            jsonObject.Add("password", "Password1!");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("/api/accounts/authenticate", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async void PostAuthenticate_MissingUserPassword_BadRequest()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient
              .DefaultRequestHeaders
              .Add("client_id", "123");
            httpClient
              .DefaultRequestHeaders
              .Add("client_secret", "123");
            httpClient
              .DefaultRequestHeaders
              .Add("grant_type", "password");

            var jsonObject = new System.Json.JsonObject();
            const string userEmail = "newuser@test.com";
            jsonObject.Add("email", userEmail);
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("/api/accounts/authenticate", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}