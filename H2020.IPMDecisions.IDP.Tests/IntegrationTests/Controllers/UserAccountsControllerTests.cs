
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
    public class UserAccountsControllerTests : IClassFixture<FakeWebHostWithDb>
    {
        private FakeWebHostWithDb fakeWebHost;
        private string myAdminToken;
        public UserAccountsControllerTests(FakeWebHostWithDb fakeWebHost)
        {
            this.fakeWebHost = fakeWebHost;

            var tokenUserId = Guid.NewGuid(); 
            var myAdminToken = TokenGeneratorTests.GenerateToken(tokenUserId, "admin"); 
        }

        [Fact]
        public async void PostUserAccount_Password_Changed_OK()
        {

            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                    new AuthenticationHeaderValue("Bearer", myAdminToken);

            httpClient
              .DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new System.Json.JsonObject();
            jsonObject.Add("currentPassword", "Password1!");
            jsonObject.Add("newPassword", "Password2!");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync(
                "/api/users/380f0a69-a009-4c34-8496-9a43c2e069be/accounts/changepassword", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<UserDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

    //TODO: Add method for current password invalid

    //TODO: Add method for userId invalid

    }
}