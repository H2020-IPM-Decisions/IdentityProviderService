using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Models;
using System.Collections.Generic;
using Microsoft.Net.Http.Headers;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Helpers;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.RegularExpressions;
using System.Linq;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IDictionary<string, object>>> CreateApplicationClient(ApplicationClientForCreationDto applicationClient, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                   out MediaTypeHeaderValue parsedMediaType))
                {
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type");
                }

                var regex = new Regex("^[a-zA-Z0-9 ]*$");
                if (!regex.IsMatch(applicationClient.Name))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Special characters are not allowed in the client name.");

                var client = await this.dataService.ApplicationClients.FindByNameAsync(applicationClient.Name);
                if (client != null)
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, string.Format("Client already exits with name: {0}", applicationClient.Name));

                var applicationClientEntity = this.mapper.Map<ApplicationClient>(applicationClient);
                CreateClientSecret(applicationClientEntity);

                this.dataService.ApplicationClients.Create(applicationClientEntity);
                await this.dataService.CompleteAsync();

                var clientToReturn = this.mapper.Map<ApplicationClientDto>(applicationClientEntity)
                    .ShapeData()
                    as IDictionary<string, object>;

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                            .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                if (includeLinks)
                {
                    var links = CreateLinksForApplicationClient(applicationClientEntity.Id);
                    clientToReturn.Add("links", links);
                }

                return GenericResponseBuilder.Success<IDictionary<string, object>>(clientToReturn);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, "Error is BLL CreateApplicationClient");
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<ApplicationClientDto>> CreateApplicationClient(Guid id, ApplicationClient applicationClient)
        {
            try
            {
                CreateClientSecret(applicationClient);
                applicationClient.Id = id;

                this.dataService.ApplicationClients.Create(applicationClient);
                await this.dataService.CompleteAsync();

                var clientToReturn = this.mapper.Map<ApplicationClientDto>(applicationClient);
                return GenericResponseBuilder.Success<ApplicationClientDto>(clientToReturn);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, "Error is BLL CreateApplicationClient");
                return GenericResponseBuilder.NoSuccess<ApplicationClientDto>(null, ex.Message.ToString());
            }
        }

        public async Task<GenericResponse> DeleteApplicationClient(Guid id)
        {
            try
            {
                var clientToDelete = await this.dataService.ApplicationClients.FindByIdAsync(id);

                if (clientToDelete == null) return GenericResponseBuilder.Success();

                this.dataService.ApplicationClients.Delete(clientToDelete);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, "Error is BLL DeleteApplicationClient");
                return GenericResponseBuilder.NoSuccess(ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<ApplicationClient>> GetApplicationClient(Guid id)
        {
            try
            {
                var entity = await this.dataService.ApplicationClients.FindByIdAsync(id);
                return GenericResponseBuilder.Success<ApplicationClient>(entity);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, "Error is BLL GetApplicationClient");
                return GenericResponseBuilder.NoSuccess<ApplicationClient>(null, ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<IDictionary<string, object>>> GetApplicationClient(Guid id, string fields, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                {
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type");
                }
                if (!propertyCheckerService.TypeHasProperties<ApplicationClientDto>(fields))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong fields entered");

                var clientFromRepository = await this.dataService.ApplicationClients.FindByIdAsync(id);
                if (clientFromRepository == null) return GenericResponseBuilder.Success<IDictionary<string, object>>(null);

                var clientToReturn = this.mapper.Map<ApplicationClientDto>(clientFromRepository)
                    .ShapeData(fields)
                    as IDictionary<string, object>;
                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                            .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                if (includeLinks)
                {
                    var links = CreateLinksForApplicationClient(id, fields);
                    clientToReturn.Add("links", links);
                }

                return GenericResponseBuilder.Success<IDictionary<string, object>>(clientToReturn);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, "Error is BLL GetApplicationClient");
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, ex.Message.ToString());
            }
        }
        public async Task<GenericResponse<ShapedDataWithLinks>> GetApplicationClients(ApplicationClientResourceParameter resourceParameter, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                    out MediaTypeHeaderValue parsedMediaType))
                {
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong media type");
                }
                if (!propertyMappingService.ValidMappingExistsFor<ApplicationClientDto, ApplicationClient>(resourceParameter.OrderBy))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong OrderBy entered");
                if (!propertyCheckerService.TypeHasProperties<ApplicationClientDto>(resourceParameter.Fields, true))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong fields entered");

                var applicationClients = await this.dataService.ApplicationClients.FindAllAsync(resourceParameter);
                if (applicationClients.Count == 0) return GenericResponseBuilder.Success<ShapedDataWithLinks>(null);

                var paginationMetaData = new PaginationMetaData()
                {
                    TotalCount = applicationClients.TotalCount,
                    PageSize = applicationClients.PageSize,
                    CurrentPage = applicationClients.CurrentPage,
                    TotalPages = applicationClients.TotalPages
                };

                var links = CreateLinksForApplicationClients(resourceParameter, applicationClients.HasNext, applicationClients.HasPrevious);

                var shapedClientsToReturn = this.mapper
                    .Map<IEnumerable<ApplicationClientDto>>(applicationClients)
                    .ShapeData(resourceParameter.Fields);

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                    .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                var shapedClientsToReturnWithLinks = shapedClientsToReturn.Select(client =>
                {
                    var userAsDictionary = client as IDictionary<string, object>;
                    if (includeLinks)
                    {
                        var userLinks = CreateLinksForApplicationClient((Guid)userAsDictionary["Id"], resourceParameter.Fields);
                        userAsDictionary.Add("links", userLinks);
                    }
                    return userAsDictionary;
                });

                var applicationClientsToReturn = new ShapedDataWithLinks()
                {
                    Value = shapedClientsToReturnWithLinks,
                    Links = links,
                    PaginationMetaData = paginationMetaData
                };

                return GenericResponseBuilder.Success<ShapedDataWithLinks>(applicationClientsToReturn);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, "Error is BLL GetApplicationClients");
                return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, ex.Message.ToString());
            }
        }

        public ApplicationClientForUpdateDto MapToApplicationClientForUpdateDto(ApplicationClient applicationClient)
        {
            return this.mapper.Map<ApplicationClientForUpdateDto>(applicationClient);
        }

        public ApplicationClient MapToApplicationClientAddingClientSecret(ApplicationClientForUpdateDto applicationClient)
        {
            var applicationClientEntity = this.mapper.Map<ApplicationClient>(applicationClient);
            CreateClientSecret(applicationClientEntity);
            return applicationClientEntity;
        }

        public async Task<GenericResponse> UpdateApplicationClient(ApplicationClient applicationClient, ApplicationClientForUpdateDto applicationClientForUpdate)
        {
            try
            {
                this.mapper.Map(applicationClientForUpdate, applicationClient);

                this.dataService.ApplicationClients.Update(applicationClient);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, "Error is BLL UpdateApplicationClient");
                return GenericResponseBuilder.NoSuccess(ex.Message.ToString());
            }
        }

        #region Helpers
        private static void CreateClientSecret(ApplicationClient applicationClientEntity)
        {
            var key = new byte[32];
            RandomNumberGenerator.Create().GetBytes(key);
            applicationClientEntity.Base64Secret = WebEncoders.Base64UrlEncode(key);
        }

        private string CreateApplicationClientResourceUri(
               ApplicationClientResourceParameter resourceParameters,
               ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return url.Link("GetApplicationClients",
                    new
                    {
                        fields = resourceParameters.Fields,
                        orderBy = resourceParameters.OrderBy,
                        pageNumber = resourceParameters.PageNumber - 1,
                        pageSize = resourceParameters.PageSize,
                        isEnabled = resourceParameters.IsEnabled,
                        searchQuery = resourceParameters.SearchQuery
                    });
                case ResourceUriType.NextPage:
                    return url.Link("GetApplicationClients",
                    new
                    {
                        fields = resourceParameters.Fields,
                        orderBy = resourceParameters.OrderBy,
                        pageNumber = resourceParameters.PageNumber + 1,
                        pageSize = resourceParameters.PageSize,
                        isEnabled = resourceParameters.IsEnabled,
                        searchQuery = resourceParameters.SearchQuery
                    });
                case ResourceUriType.Current:
                default:
                    return url.Link("GetApplicationClients",
                    new
                    {
                        fields = resourceParameters.Fields,
                        orderBy = resourceParameters.OrderBy,
                        pageNumber = resourceParameters.PageNumber,
                        pageSize = resourceParameters.PageSize,
                        isEnabled = resourceParameters.IsEnabled,
                        searchQuery = resourceParameters.SearchQuery
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForApplicationClients(
            ApplicationClientResourceParameter resourceParameters,
            bool hasNextPage,
            bool hasPreviousPage)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                CreateApplicationClientResourceUri(resourceParameters, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNextPage)
            {
                links.Add(new LinkDto(
                CreateApplicationClientResourceUri(resourceParameters, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }
            if (hasPreviousPage)
            {
                links.Add(new LinkDto(
               CreateApplicationClientResourceUri(resourceParameters, ResourceUriType.PreviousPage),
               "previous_page",
               "GET"));
            }
            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForApplicationClient(
            Guid id,
            string fields = "")
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(
                url.Link("GetApplicationClient", new { id }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                 url.Link("GetApplicationClient", new { id, fields }),
                 "self",
                 "GET"));
            }

            links.Add(new LinkDto(
                url.Link("DeleteApplicationClient", new { id }),
                "delete_application_client",
                "DELETE"));

            links.Add(new LinkDto(
                url.Link("PartialUpdateApplicationClient", new { id }),
                "update_application_client",
                "PATCH"));

            return links;
        }
        #endregion
    }
}