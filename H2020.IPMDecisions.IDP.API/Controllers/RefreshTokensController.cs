using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using System;
using System.Text.Json;
using H2020.IPMDecisions.IDP.BLL;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Authorize(Roles = "Admin", AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("/api/refreshtokens")]
    public class RefreshTokensController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public RefreshTokensController(
            IBusinessLogic businessLogic)
        {           
            this.businessLogic = businessLogic
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json, "application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpGet("", Name = "GetRefreshTokens")]
        [HttpHead]
        // GET: api/refreshtokens
        public async Task<IActionResult> Get(
            [FromQuery] RefreshTokenResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetRefreshTokens(resourceParameter, mediaType);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            if (response.Result == null)
                return NotFound();

            Response.Headers.Append("X-Pagination",
                JsonSerializer.Serialize(response.Result.PaginationMetaData));

            return Ok(
                new
                {
                    value = response.Result.Value,
                    links = response.Result.Links
                }
            );
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json, "application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpGet("{id:guid}", Name = "GetRefreshToken")]
        // GET: api/refreshtokens/1
        public async Task<IActionResult> Get(
            [FromRoute] Guid id,
            [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {

            var response = await this.businessLogic.GetRefreshToken(id, fields, mediaType);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            if (response.Result == null)
                return NotFound();

            return Ok(response.Result);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "DeleteRefreshToken")]
        //DELETE :  api/refreshtokens/1
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await this.businessLogic.DeleteRefreshToken(id);

            if (!response.IsSuccessful)
            {
                return BadRequest(new { message = response.ErrorMessage });
            }
            return NoContent();
        }
    }
}