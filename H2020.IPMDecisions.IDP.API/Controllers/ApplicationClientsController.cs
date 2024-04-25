using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using H2020.IPMDecisions.IDP.BLL;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Authorize(Roles = "Admin", AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("/api/applicationclients")]
    public class ApplicationClientsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public ApplicationClientsController(
            IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json, "application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpGet("", Name = "GetApplicationClients")]
        [HttpHead]
        //GET: api/applicationclients
        public async Task<IActionResult> GetApplicationClients(
            [FromQuery] ApplicationClientResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetApplicationClients(resourceParameter, mediaType);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            if (response.Result == null)
                return NotFound();

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(response.Result.PaginationMetaData));

            return Ok(
                new
                {
                    value = response.Result.Value,
                    links = response.Result.Links
                }
            );
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json, "application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpPost]
        [Route("")]
        //POST: api/applicationclients
        public async Task<ActionResult<ApplicationClientDto>> Post(
            [FromBody] ApplicationClientForCreationDto clientForCreationDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.CreateApplicationClient(clientForCreationDto, mediaType);

            if (!response.IsSuccessful)
            {
                return BadRequest(new { message = response.ErrorMessage });
            }
            return CreatedAtRoute("GetApplicationClient",
                 new { id = response.Result["Id"] },
                 response.Result);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "DeleteApplicationClient")]
        //DELETE: api/applicationclients/1
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await this.businessLogic.DeleteApplicationClient(id);

            if (!response.IsSuccessful)
            {
                return BadRequest(new { message = response.ErrorMessage });
            }
            return NoContent();
        }

        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id:guid}", Name = "PartialUpdateApplicationClient")]
        //PATCH: api/applicationclients/1
        public async Task<IActionResult> PartialUpdate(
            [FromRoute] Guid id,
            JsonPatchDocument<ApplicationClientForUpdateDto> patchDocument)
        {
            var applicationClientResponse = await this.businessLogic.GetApplicationClient(id);

            if (applicationClientResponse.Result == null)
            {
                var clientDto = new ApplicationClientForUpdateDto();
                patchDocument.ApplyTo(clientDto, ModelState);
                if (!TryValidateModel(clientDto))
                    return ValidationProblem(ModelState);

                var applicationClientWithSecret = this.businessLogic.MapToApplicationClientAddingClientSecret(clientDto);                
                if (!TryValidateModel(applicationClientWithSecret))
                    return ValidationProblem(ModelState);

                var createClientResponse = await this.businessLogic.CreateApplicationClient(id, applicationClientWithSecret);

                if(!createClientResponse.IsSuccessful)
                    return BadRequest(new { message = createClientResponse.ErrorMessage });

                return CreatedAtRoute("GetApplicationClient",
                 new { id = id },
                 createClientResponse.Result);
            }
            var clientToPatch = this.businessLogic.MapToApplicationClientForUpdateDto(applicationClientResponse.Result);
            patchDocument.ApplyTo(clientToPatch, ModelState);
            if (!TryValidateModel(clientToPatch))
                return ValidationProblem(ModelState);

            var response = await this.businessLogic.UpdateApplicationClient(applicationClientResponse.Result, clientToPatch);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();            
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json, "application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpGet("{id:guid}", Name = "GetApplicationClient")]
        //GET: api/applicationclients/1
        public async Task<IActionResult> Get(
            [FromRoute] Guid id,
            [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetApplicationClient(id, fields, mediaType);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            if (response.Result == null)
                return NotFound();

            return Ok(response.Result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/applicationclients
        public IActionResult Options()
        {
            Response.Headers.Append("Allow", "OPTIONS,POST,GET,DELETE,PATCH");
            return Ok();
        }
    }
}