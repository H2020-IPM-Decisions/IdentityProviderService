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
                    .WithPath("/api/eml/internal/registrationemail")                    
                    .WithHeader("ipm-internal-auth", "1234")
                    //.WithHeader("Content-Type", "application/vnd.h2020ipmdecisions.email+json")
                    .WithBody(new WildcardMatcher("*newuser*"))
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json"));

            stub.Given(
                Request.Create()
                    .WithPath("/api/eml/internal/registrationemail")
                    .WithHeader("ipm-internal-auth", "1234")
                    //.WithHeader("Content-Type", "application/vnd.h2020ipmdecisions.email+json")
                    .WithBody(new WildcardMatcher("*emailservicedown*"))
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(400)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""message"": ""No connection could be made because the target machine actively refused it."" }"));

            stub.Given(
                Request.Create()
                    .WithPath("/api/eml/internal/forgotpassword")
                    .WithHeader("ipm-internal-auth", "1234")
                    //.WithHeader("Content-Type", "application/vnd.h2020ipmdecisions.email+json")
                    .WithBody(new WildcardMatcher("*"))
                    .UsingPost())
                .AtPriority(10)
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json"));

            stub.Given(
                Request.Create()
                    .WithPath("/api/eml/internal/forgotpassword")
                    .WithHeader("ipm-internal-auth", "1234")
                    //.WithHeader("Content-Type", "application/vnd.h2020ipmdecisions.email+json")
                    .WithBody(new WildcardMatcher("*failemail*"))
                    .UsingPost())
                .AtPriority(1)
                .RespondWith(Response.Create()
                    .WithStatusCode(400)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""message"": ""No connection could be made because the target machine actively refused it."" }"));

            stub.Given(
                Request.Create()
                    .WithPath("/api/upr/internal/userprofile")                    
                    .WithHeader("ipm-internal-auth", "1234")
                    //.WithHeader("Content-Type", "application/vnd.h2020ipmdecisions.email+json")
                    .WithBody(new WildcardMatcher("*"))
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200));
        }

        public void Dispose()
        {
            stub.Stop();
        }
    }

    public class FakeApiGatewayHostTests : IClassFixture<FakeApiGatewayHost>
    {
        private readonly FakeApiGatewayHost fixture;

        public FakeApiGatewayHostTests(FakeApiGatewayHost fixture)
        {
            this.fixture = fixture;
        }
    }
}