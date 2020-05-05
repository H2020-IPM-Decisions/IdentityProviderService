
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
    public class UserProfilesControllerTests : IClassFixture<FakeWebHostWithDb>
    {
        private FakeWebHostWithDb fakeWebHost;
        public UserProfilesControllerTests(FakeWebHostWithDb fakeWebHost)
        {
            this.fakeWebHost = fakeWebHost;
        }

        [Fact]
        public async void Post_ValidCall_Created()
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
            var responseDeserialized = JsonConvert.DeserializeObject<UserDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseDeserialized.Email.Should().Be(userEmail);

        }    
    }
}