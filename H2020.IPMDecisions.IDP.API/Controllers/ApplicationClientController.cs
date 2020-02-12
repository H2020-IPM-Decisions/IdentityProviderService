using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Helpers;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using H2020.IPMDecisions.IDP.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Mime;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "SuperAdmin", AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("/api/applicationclient")]
    public class ApplicationClientController : ControllerBase
    {
        private readonly IDataService dataService;
        private readonly IMapper mapper;
        private readonly IPropertyMappingService propertyMappingService;
        private readonly IPropertyCheckerService propertyCheckerService;

        public ApplicationClientController(
            IDataService dataService, 
            IMapper mapper,
            IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            this.dataService = dataService
                ?? throw new System.ArgumentNullException(nameof(dataService));
            this.mapper = mapper 
                ?? throw new ArgumentNullException(nameof(mapper));
            this.propertyMappingService = propertyMappingService 
                ?? throw new ArgumentNullException(nameof(propertyMappingService));
            this.propertyCheckerService = propertyCheckerService 
                ?? throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [HttpGet("", Name = "GetApplicationClients")]
        [HttpHead]
        // GET: api/applicationclient
        public async Task<IActionResult> GetApplicationClients(
            [FromQuery] ApplicationClientResourceParameter resourceParameter)
        {
            if (!propertyMappingService.ValidMappingExistsFor<ApplicationClientDto, ApplicationClient>(resourceParameter.OrderBy))
                return BadRequest();
            if (!propertyCheckerService.TypeHasProperties<ApplicationClientDto>(resourceParameter.Fields, true))
                return BadRequest();

            var applicationClients = await this.dataService.ApplicationClients.FindAllAsync(resourceParameter);
            if (applicationClients.Count() == 0) return NotFound();

            var paginationMetaData = new
            {
                totalCount = applicationClients.TotalCount,
                pageSize = applicationClients.PageSize,
                currentPage = applicationClients.CurrentPage,
                totalPages = applicationClients.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetaData));

            var links = CreateLinksForApplicationClients(resourceParameter, applicationClients.HasNext, applicationClients.HasPrevious);

            var shapedClientsToReturn = this.mapper
                .Map<IEnumerable<ApplicationClientDto>>(applicationClients)
                .ShapeData(resourceParameter.Fields);           

            var shapedClientsToReturnWithLinks = shapedClientsToReturn.Select(client =>
            {
                var userAsDictionary = client as IDictionary<string, object>;
                var userLinks = CreateLinksForApplicationClient((Guid)userAsDictionary["Id"], resourceParameter.Fields);
                userAsDictionary.Add("links", userLinks);
                return userAsDictionary;
            });

            var applicationClientsToReturn = new
            {
                value = shapedClientsToReturnWithLinks,
                links
            };

            return Ok(applicationClientsToReturn);
        }

        [HttpPost]
        [Route("")]
        // POST: api/applicationclient
        public async Task<ActionResult<ApplicationClientDto>> Post(
            [FromBody] ApplicationClientForCreationDto clientForCreationDto)
        {
            var regex = new Regex("^[a-zA-Z0-9 ]*$");
            if (!regex.IsMatch(clientForCreationDto.Name))
                return BadRequest("Special characters are not allowed in the client name.");

            var client = await this.dataService.ApplicationClients.FindByNameAsync(clientForCreationDto.Name);
            if (client != null)
                return BadRequest(string.Format("Client already exits with name: {0}", clientForCreationDto.Name));

            var applicationClientEntity = this.mapper.Map<ApplicationClient>(clientForCreationDto);
            CreateClientSecret(applicationClientEntity);
            
            this.dataService.ApplicationClients.Create(applicationClientEntity);
            await this.dataService.CompleteAsync();

            var links = CreateLinksForApplicationClient(applicationClientEntity.Id);
            var clientToReturn = this.mapper.Map<ApplicationClientDto>(applicationClientEntity)
                .ShapeData()
                as IDictionary<string, object>; ;
            clientToReturn.Add("links", links);

            return CreatedAtRoute("GetApplicationClient",
                 new { id = clientToReturn["Id"] },
                 clientToReturn);
        }       

        [HttpDelete("{id:guid}", Name = "DeleteApplicationClient")]
        //DELETE :  api/applicationclient/1
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var clientToDelete = await this.dataService.ApplicationClients.FindByIdAsync(id);

            if (clientToDelete == null) return NotFound();

            this.dataService.ApplicationClients.Delete(clientToDelete);
            await this.dataService.CompleteAsync();

            return NoContent();
        }

        [HttpPatch("{id:guid}", Name = "PartialUpdateApplicationClient")]
        //PATCH :  api/applicationclient/1
        public async Task<IActionResult> PartialUpdate(
            [FromRoute] Guid id,
            JsonPatchDocument<ApplicationClientForUpdateDto> patchDocument)
        {
            var clientFromRepository = await this.dataService.ApplicationClients.FindByIdAsync(id);

            if (clientFromRepository == null)
            {
                var clientDto = new ApplicationClientForUpdateDto();
                patchDocument.ApplyTo(clientDto, ModelState);

                if (!TryValidateModel(clientDto))
                    return ValidationProblem(ModelState);

                var clientToAdd = this.mapper.Map<ApplicationClient>(clientDto);
                clientToAdd.Id = id;
                CreateClientSecret(clientToAdd);

                this.dataService.ApplicationClients.Create(clientToAdd);
                await this.dataService.CompleteAsync();

                var clientToReturn = this.mapper.Map<ApplicationClientDto>(clientToAdd);
                return CreatedAtRoute("GetApplicationClient",
                 new { id = id },
                 clientToReturn);
            }

            var clientToPatch = this.mapper.Map<ApplicationClientForUpdateDto>(clientFromRepository);
            // ToDo need validation
            patchDocument.ApplyTo(clientToPatch, ModelState);

            if (!TryValidateModel(clientToPatch))
                return ValidationProblem(ModelState);

            this.mapper.Map(clientToPatch, clientFromRepository);

            this.dataService.ApplicationClients.Update(clientFromRepository);
            await this.dataService.CompleteAsync();
                        
            return NoContent();
        }

        [HttpGet("{id:guid}", Name = "GetApplicationClient")]
        // GET: api/applicationclient/1
        public async Task<IActionResult> Get([FromRoute] Guid id, [FromQuery] string fields)
        {
            if (!propertyCheckerService.TypeHasProperties<ApplicationClientDto>(fields))
                return BadRequest();

            var clientFromRepository = await this.dataService.ApplicationClients.FindByIdAsync(id);
            if (clientFromRepository == null) return NotFound();

            var links = CreateLinksForApplicationClient(id, fields);
            var clientToReturn = this.mapper.Map<ApplicationClientDto>(clientFromRepository)
                .ShapeData(fields)
                as IDictionary<string, object>; ;
            clientToReturn.Add("links", links);

            return Ok(clientToReturn);
        }

        [HttpOptions]
        // OPTIONS: api/applicationclient
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST,GET,DELETE,PATCH");
            return Ok();
        }

        #region helpers
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
                    return Url.Link("GetApplicationClients",
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
                    return Url.Link("GetApplicationClients",
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
                    return Url.Link("GetApplicationClients",
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
                Url.Link("GetApplicationClient", new { id }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                 Url.Link("GetApplicationClient", new { id, fields }),
                 "self",
                 "GET"));
            }

            links.Add(new LinkDto(
                Url.Link("GetApplicationClient", new { id }),
                "self",
                "GET"));

            links.Add(new LinkDto(
                Url.Link("DeleteApplicationClient", new { id }),
                "delete_application_client",
                "DELETE"));

            links.Add(new LinkDto(
                Url.Link("PartialUpdateApplicationClient", new { id }),
                "update_application_client",
                "PATCH"));

            return links;
        }
        #endregion
    }
}