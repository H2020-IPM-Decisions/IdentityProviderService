using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
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

            var key = new byte[32];
            RandomNumberGenerator.Create().GetBytes(key);
            applicationClientEntity.Base64Secret = WebEncoders.Base64UrlEncode(key);
            
            this.dataService.ApplicationClients.Create(applicationClientEntity);
            await this.dataService.CompleteAsync();

            var clientToReturn = this.mapper.Map<ApplicationClientDto>(applicationClientEntity);
            return Ok(clientToReturn);
        }
    }
}