
using System;
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
    public class UserAccountsControllerTests : IClassFixture<FakeWebHostWithDb>
    {
        private FakeWebHostWithDb fakeWebHost;
        private string myDefaultUserToken;
        private Guid defaultUserId;
        public UserAccountsControllerTests(FakeWebHostWithDb fakeWebHost)
        {
            this.fakeWebHost = fakeWebHost;

            defaultUserId = Guid.Parse(fakeWebHost.DefaultNormalUserID.ToString());
            myDefaultUserToken = TokenGeneratorTests.GenerateToken(defaultUserId);           
        }

        [Fact]
        public async void PostChangePassword_RightPassword_OK()
        {

            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                    new AuthenticationHeaderValue("Bearer", myDefaultUserToken);

            httpClient
              .DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new System.Json.JsonObject();
            jsonObject.Add("currentPassword", fakeWebHost.DefaultUserPassword.ToString());
            jsonObject.Add("newPassword", "Password2!");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync(
                string.Format("api/users/{0}/accounts/changepassword", defaultUserId), content);
            
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void PostChangePassword_WrongPassword_BadRequest()
        {

            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                    new AuthenticationHeaderValue("Bearer", myDefaultUserToken);

            httpClient
              .DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new System.Json.JsonObject();
            jsonObject.Add("currentPassword", "ThisIsWrong");
            jsonObject.Add("newPassword", "Password2!");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync(
                string.Format("api/users/{0}/accounts/changepassword", defaultUserId), content);


            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async void PostChangePassword_DifferentUserIdUrlAndToken_Unauthorized()
        {

            // Arrange
            var newUserId = Guid.NewGuid();

            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                    new AuthenticationHeaderValue("Bearer", myDefaultUserToken);

            httpClient
              .DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new System.Json.JsonObject();
            jsonObject.Add("currentPassword", fakeWebHost.DefaultUserPassword.ToString());
            jsonObject.Add("newPassword", "Password2!");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync(
                string.Format("api/users/{0}/accounts/changepassword", newUserId), content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async void PostChangePassword_UserDontExist_NotFound()
        {
            // Arrange
            var notExistingUserId = Guid.NewGuid();
            var notExistingUserIdToken = TokenGeneratorTests.GenerateToken(notExistingUserId);

            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                    new AuthenticationHeaderValue("Bearer", notExistingUserIdToken);

            httpClient
              .DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new System.Json.JsonObject();
            jsonObject.Add("currentPassword", fakeWebHost.DefaultUserPassword.ToString());
            jsonObject.Add("newPassword", "Password2!");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync(
                string.Format("api/users/{0}/accounts/changepassword", notExistingUserId), content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void PostChangePassword_AdminChangeUsersPassword_Ok()
        {
            // Arrange           
            string myAdminUserToken = TokenGeneratorTests.GenerateToken(Guid.NewGuid(), "admin");

            // Create new WebHost as user has already changed password
            var newFakeWebHost = new FakeWebHostWithDb();
            newFakeWebHost.IsDatabaseInitialized = false;
            await newFakeWebHost.InitializeAsync();
            var httpClient = newFakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                    new AuthenticationHeaderValue("Bearer", myAdminUserToken);

            httpClient
              .DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new System.Json.JsonObject();
            jsonObject.Add("currentPassword", newFakeWebHost.DefaultUserPassword.ToString());
            jsonObject.Add("newPassword", "Password2!");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync(
                string.Format("api/users/{0}/accounts/changepassword", defaultUserId), content);
            var responseContent = await response.Content.ReadAsStringAsync();

            await newFakeWebHost.DisposeAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}