using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using H2020.IPMDecisions.IDP.Core.Helpers;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using System.Linq;
using System;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "SuperAdmin", AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("/api/refreshtokens")]
    public class RefreshTokensController : ControllerBase
    {
        private readonly IDataService dataService;
        private readonly IMapper mapper;

        public RefreshTokensController(
            IDataService dataService,
            IMapper mapper)
        {
            this.dataService = dataService
                ?? throw new System.ArgumentNullException(nameof(dataService));
            this.mapper = mapper
                ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("", Name = "GetRefreshTokens")]
        [HttpHead]
        // GET: api/refreshtokens
        public async Task<IActionResult> Get()
        {
            var refreshTokens = await this.dataService.RefreshTokens.FindAllAsync();
            if (refreshTokens.Count() == 0) return NotFound();
            
            var refreshtokensToReturn = this.mapper
                .Map<IEnumerable<RefreshTokenDto>>(refreshTokens);

            return Ok(refreshtokensToReturn);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:guid}", Name = "GetRefreshToken")]
        // GET: api/applicationclient/1
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var refreshTokenFromRepository = await this.dataService.RefreshTokens.FindByIdAsync(id);
            if (refreshTokenFromRepository == null) return NotFound();

            var clientToReturn = this.mapper.Map<RefreshTokenDto>(refreshTokenFromRepository);
            return Ok(clientToReturn);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:guid}", Name = "DeleteRefreshToken")]
        //DELETE :  api/refreshtokens/1
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var refreshTokenToDelete = await this.dataService.RefreshTokens.FindByIdAsync(id);

            if (refreshTokenToDelete == null) return NotFound();

            this.dataService.RefreshTokens.Delete(refreshTokenToDelete);
            await this.dataService.CompleteAsync();

            return NoContent();
        }


    }
}