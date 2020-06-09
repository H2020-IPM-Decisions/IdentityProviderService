using System;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace H2020.IPMDecisions.IDP.Tests
{
    public class FakeApiGatewayHost : IDisposable
    {
        private WireMockServer stub;
        public FakeApiGatewayHost()
        {
            stub = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://+:5003" },
                StartAdminInterface = true
            });


            stub.Given(
                Request.Create()
                    .WithPath("/api/eml/accounts/registrationemail")
                    .WithHeader("ipm-eml-auth", "1234")
                    //.WithHeader("Content-Type", "application/vnd.h2020ipmdecisions.email+json")
                    .WithBody(new WildcardMatcher("*newuser@test.com*"))
                    .UsingPost()                   
                )
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json"));
        }

        public void Dispose()
        {
            stub.Stop();
        }
    }

    public class FakeApiGatewayHostTests : IClassFixture<FakeApiGatewayHost>
    {
        FakeApiGatewayHost fixture;

        public FakeApiGatewayHostTests(FakeApiGatewayHost fixture)
        {
            this.fixture = fixture;
        }
    }
}