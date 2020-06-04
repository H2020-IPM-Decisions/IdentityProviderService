using System;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Data.Persistence;
using H2020.IPMDecisions.IDP.Data.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace H2020.IPMDecisions.IDP.Tests.UnitTests.RepositoryTests
{
    public class ApplicationClientRepositoryTests
    {
        [Fact]
        public async void FindAll_PageSizeIs2_Returns2ApplicationClients()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"DatabaseForTesting{Guid.NewGuid()}")
                .Options;

            var resourceParameter = new ApplicationClientResourceParameter()
            {
                PageSize = 2,
            };

            using (var context = new ApplicationDbContext(options))
            {
                context.ApplicationClient.Add(new ApplicationClient()
                { Name = "1", JWTAudienceCategory = "1.com" });
                context.ApplicationClient.Add(new ApplicationClient()
                { Name = "1", JWTAudienceCategory = "1.com" });
                context.ApplicationClient.Add(new ApplicationClient()
                { Name = "1", JWTAudienceCategory = "1.com" });

                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var repository = new ApplicationClientRepository(context);
                // Act
                var clients = await repository.FindAllAsync(resourceParameter);

                // Assert
                Assert.Equal(2, clients.Count);
            };
        }

        [Fact]
        public async void FindAll_3RecordsPageSizeIs2AndPageNumber2_Returns1ApplicationClients()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"DatabaseForTesting{Guid.NewGuid()}")
                .Options;

            var resourceParameter = new ApplicationClientResourceParameter()
            {
                PageSize = 2,
                PageNumber = 2
            };

            using (var context = new ApplicationDbContext(options))
            {
                context.ApplicationClient.Add(new ApplicationClient()
                { Name = "1", JWTAudienceCategory = "1.com" });
                context.ApplicationClient.Add(new ApplicationClient()
                { Name = "1", JWTAudienceCategory = "1.com" });
                context.ApplicationClient.Add(new ApplicationClient()
                { Name = "1", JWTAudienceCategory = "1.com" });

                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var repository = new ApplicationClientRepository(context);
                // Act
                var clients = await repository.FindAllAsync(resourceParameter);

                // Assert
                Assert.Single(clients);
            };
        }

        [Fact]
        public async void GetApplicationByName_ValidName_ReturnsApplication()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"DatabaseForTesting{Guid.NewGuid()}")
                .Options;

            var applicationToReturn = new ApplicationClient()
            { Name = "1", JWTAudienceCategory = "1.com" };

            using (var context = new ApplicationDbContext(options))
            {
                context.ApplicationClient.Add(applicationToReturn);

                context.ApplicationClient.Add(new ApplicationClient()
                { Name = "2", JWTAudienceCategory = "3.com" });
                context.ApplicationClient.Add(new ApplicationClient()
                { Name = "3", JWTAudienceCategory = "3.com" });

                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var repository = new ApplicationClientRepository(context);

                // Act
                var clientReturned = await repository.FindByNameAsync("1");

                // Assert
                Assert.Equal(applicationToReturn.Name, clientReturned.Name);
            };
        }
    }
}