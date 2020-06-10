
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using H2020.IPMDecisions.IDP.Core.Dtos;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace H2020.IPMDecisions.IDP.Tests.IntegrationTests.Controllers
{
    [Trait("Category", "Docker")]
    public class AccountsControllerTests : IClassFixture<FakeWebHostWithDb>, IClassFixture<FakeApiGatewayHost>
    {
        private FakeWebHostWithDb fakeWebHost;
        public AccountsControllerTests(FakeWebHostWithDb fakeWebHost)
        {
            this.fakeWebHost = fakeWebHost;
        }

        [Fact]
        public async void Post_RegisterValidCall_Ok()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
              .DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new System.Json.JsonObject();
            const string userEmail = "newuser@test.com";
            jsonObject.Add("email", userEmail);
            jsonObject.Add("password", "Password1!");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/accounts/register", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<UserRegistrationReturnDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseDeserialized.Email.Should().Be(userEmail);
            responseDeserialized.EmailSentDuringRegistration.Should().Be(true);
        }

        [Fact]
        public async void Post_RegisterEmailServiceDown_OK()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
              .DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new System.Json.JsonObject();
            const string userEmail = "emailservicedown@test.com";
            jsonObject.Add("email", userEmail);
            jsonObject.Add("password", "Password1!");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/accounts/register", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<UserRegistrationReturnDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseDeserialized.Email.Should().Be(userEmail);
            responseDeserialized.EmailSentDuringRegistration.Should().Be(false);
        }

        [Fact]
        public async void PostAuthenticate_ValidCall_Ok()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient
              .DefaultRequestHeaders
              .Add("client_id", fakeWebHost.DefaultApplicationClientId.ToString());
            httpClient
              .DefaultRequestHeaders
              .Add("client_secret", fakeWebHost.DefaultApplicationClientSecret.ToString());
            httpClient
              .DefaultRequestHeaders
              .Add("grant_type", "password");

            var jsonObject = new System.Json.JsonObject();
            string userEmail = fakeWebHost.DefaultNormalUserEmail.ToString();
            jsonObject.Add("email", userEmail);
            jsonObject.Add("password", fakeWebHost.DefaultUserPassword.ToString());
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("/api/accounts/authenticate", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void PostAuthenticate_InvalidPassword_BadRequest()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient
              .DefaultRequestHeaders
              .Add("client_id", fakeWebHost.DefaultApplicationClientId.ToString());
            httpClient
              .DefaultRequestHeaders
              .Add("client_secret", fakeWebHost.DefaultApplicationClientSecret.ToString());
            httpClient
              .DefaultRequestHeaders
              .Add("grant_type", "password");

            var jsonObject = new System.Json.JsonObject();
            string userEmail = fakeWebHost.DefaultNormalUserEmail.ToString();
            jsonObject.Add("email", userEmail);
            jsonObject.Add("password", "BadPassword");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("/api/accounts/authenticate", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("Username or password is incorrect");
        }

        [Fact]
        public async void PostAuthenticate_InvalidClientId_BadRequest()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient
              .DefaultRequestHeaders
              .Add("client_id", new Guid().ToString());
            httpClient
              .DefaultRequestHeaders
              .Add("client_secret", fakeWebHost.DefaultApplicationClientSecret.ToString());
            httpClient
              .DefaultRequestHeaders
              .Add("grant_type", "password");

            var jsonObject = new System.Json.JsonObject();
            string userEmail = fakeWebHost.DefaultNormalUserEmail.ToString();
            jsonObject.Add("email", userEmail);
            jsonObject.Add("password", fakeWebHost.DefaultUserPassword.ToString());
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
        public async void PostAuthenticate_PublicApplicationClientClientSecretNotNeeded_Ok()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient
              .DefaultRequestHeaders
              .Add("client_id", fakeWebHost.DefaultApplicationClientId.ToString());
            httpClient
              .DefaultRequestHeaders
              .Add("client_secret", "needsTohaveSomething");
            httpClient
              .DefaultRequestHeaders
              .Add("grant_type", "password");

            var jsonObject = new System.Json.JsonObject();
            string userEmail = fakeWebHost.DefaultNormalUserEmail.ToString();
            jsonObject.Add("email", userEmail);
            jsonObject.Add("password", fakeWebHost.DefaultUserPassword.ToString());
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("/api/accounts/authenticate", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void PostAuthenticate_ConfidentialApplicationClientNeedsClientSecret_BadRequest()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient
              .DefaultRequestHeaders
              .Add("client_id", fakeWebHost.ConfidentialApplicationClientId.ToString());
            httpClient
              .DefaultRequestHeaders
              .Add("client_secret", "invalidSecret");
            httpClient
              .DefaultRequestHeaders
              .Add("grant_type", "password");

            var jsonObject = new System.Json.JsonObject();
            string userEmail = fakeWebHost.DefaultNormalUserEmail.ToString();
            jsonObject.Add("email", userEmail);
            jsonObject.Add("password", fakeWebHost.DefaultUserPassword.ToString());
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("/api/accounts/authenticate", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("Client secret do not match");
        }

        [Fact]
        public async void PostAuthenticate_DisableApplicationClient_BadRequest()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient
              .DefaultRequestHeaders
              .Add("client_id", fakeWebHost.DisableApplicationClientId.ToString());
            httpClient
              .DefaultRequestHeaders
              .Add("client_secret", fakeWebHost.DefaultApplicationClientSecret.ToString());
            httpClient
              .DefaultRequestHeaders
              .Add("grant_type", "password");

            var jsonObject = new System.Json.JsonObject();
            string userEmail = fakeWebHost.DefaultNormalUserEmail.ToString();
            jsonObject.Add("email", userEmail);
            jsonObject.Add("password", fakeWebHost.DefaultUserPassword.ToString());
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("/api/accounts/authenticate", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("Client is inactive");
        }

        [Fact]
        public async void PostForgotPassword_ValidCall_OK()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
              .DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new System.Json.JsonObject();
            string userEmail = fakeWebHost.DefaultNormalUserEmail.ToString();
            jsonObject.Add("email", userEmail);           
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/accounts/ForgotPassword", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void PostForgotPassword_UserDoesntExist_Ok()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
              .DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new System.Json.JsonObject();
            string userEmail = "Idontexist@test.com";
            jsonObject.Add("email", userEmail);           
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/accounts/ForgotPassword", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void PostForgotPassword_EmailServiceDown_BadRequest()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
              .DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new System.Json.JsonObject();
            string userEmail = fakeWebHost.DefaultFailEmailUserEmail.ToString();
            jsonObject.Add("email", userEmail);
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/accounts/ForgotPassword", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("Email send failed");
        }
    }
}