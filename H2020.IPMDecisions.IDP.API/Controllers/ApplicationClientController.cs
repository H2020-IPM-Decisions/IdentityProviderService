using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> GetApplicationClients()
        {
            var applicationClients = await this.dataService.ApplicationClients.FindAllAsync();

            var applicationClientsToReturn = this.mapper.Map<List<ApplicationClientDto>>(applicationClients);

            return Ok(applicationClients);
        }

        [HttpPost]
        [Route("")]
        // POST: api/applicationclient
        public async Task<ActionResult<ApplicationClientDto>> Post(
            [FromBody] ApplicationClientForCreationDto clientForCreationDto)
        {

            var regex = new Regex("^[a-zA-Z0-9 ]*$");
            if (!regex.IsMatch(clientForCreationDto.Name))
                return BadRequest("Special characteres are not allowed in the client name.");

            var client = await this.dataService.ApplicationClients.FindByNameAsync(clientForCreationDto.Name);

            if (client != null)
                return BadRequest(string.Format("Client already exits with name: {0}", clientForCreationDto.Name));

            var applicationClientEntity = this.mapper.Map<ApplicationClient>(clientForCreationDto);
            CreateClientSecret(applicationClientEntity);

            this.dataService.ApplicationClients.Create(applicationClientEntity);
            await this.dataService.CompleteAsync();

            var clientToReturn = this.mapper.Map<ApplicationClientDto>(applicationClientEntity);
            return Ok(clientToReturn);
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
        //Patch :  api/applicationclient/1
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
                return Ok(clientToReturn);
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

        private static void CreateClientSecret(ApplicationClient applicationClientEntity)
        {
            var key = new byte[32];
            RandomNumberGenerator.Create().GetBytes(key);
            applicationClientEntity.Base64Secret = WebEncoders.Base64UrlEncode(key);
        }
    }
}