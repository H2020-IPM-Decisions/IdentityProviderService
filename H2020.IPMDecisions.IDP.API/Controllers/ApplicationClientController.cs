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

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [ApiController]
    [Route("/api/applicationclient")]
    public class ApplicationClientController : ControllerBase
    {
        private readonly IDataService dataService;
        private readonly IMapper mapper;

        public ApplicationClientController(
            IDataService dataService, 
            IMapper mapper)
        {
            this.dataService = dataService
                ?? throw new System.ArgumentNullException(nameof(dataService));
            this.mapper = mapper 
                ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("", Name = "GetApplicationClients")]
        [HttpHead]
        // GET: api/applicationclient
        public async Task<IActionResult> GetApplicationClients(
            [FromQuery] ApplicationClientResourceParameter resourceParameter)
        {
            var applicationClients = await this.dataService.ApplicationClients.FindAllAsync(resourceParameter);
            if (applicationClients.Count() == 0) return NotFound();

            var previousPageLink = applicationClients.HasPrevious ?
                CreateApplicationClientResourceUri(resourceParameter,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = applicationClients.HasNext ?
                CreateApplicationClientResourceUri(resourceParameter,
                ResourceUriType.NextPage) : null;

            var paginationMetaData = new
            {
                totalCount = applicationClients.TotalCount,
                pageSize = applicationClients.PageSize,
                currentPage = applicationClients.CurrentPage,
                totalPages = applicationClients.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetaData));

            var applicationClientsToReturn = this.mapper.Map<List<ApplicationClientDto>>(applicationClients);           

            var applicationClientsToReturnWithLinks = applicationClientsToReturn.Select(client =>
            {
                var userAsDictionary = client.ShapeData() as IDictionary<string, object>;
                var userLinks = CreateLinksForApplicationClient((Guid)userAsDictionary["Id"]);
                userAsDictionary.Add("links", userLinks);
                return userAsDictionary;
            });

            return Ok(applicationClientsToReturnWithLinks);
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
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var clientFromRepository = await this.dataService.ApplicationClients.FindByIdAsync(id);
            if (clientFromRepository == null) return NotFound();

            var links = CreateLinksForApplicationClient(id);
            var clientToReturn = this.mapper.Map<ApplicationClientDto>(clientFromRepository)
                .ShapeData()
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
                        pageNumber = resourceParameters.PageNumber - 1,
                        pageSize = resourceParameters.PageSize,
                        isEnabled = resourceParameters.IsEnabled,
                        searchQuery = resourceParameters.SearchQuery
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetApplicationClients",
                    new
                    {
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
                        pageNumber = resourceParameters.PageNumber,
                        pageSize = resourceParameters.PageSize,
                        isEnabled = resourceParameters.IsEnabled,
                        searchQuery = resourceParameters.SearchQuery
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForApplicationClient(
            Guid id)
        {
            var links = new List<LinkDto>();

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