using System;
using System.Net.Mime;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.API.Filters;
using H2020.IPMDecisions.IDP.BLL;
using H2020.IPMDecisions.IDP.Core.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/internalcall")]
    [Consumes("application/vnd.h2020ipmdecisions.internal+json")]
    [TypeFilter(typeof(RequestHasTokenResourceFilter))]
    public class InternalCallsController: ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public InternalCallsController(
            IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic ??
                throw new ArgumentNullException(nameof(businessLogic));
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("getuserid", Name = "api.post.internal.id")]
        // POST: api/internalcall/getuserid
        public async Task<IActionResult> Post(
            [FromBody] UserEmailDto userEmailDto)
        {
            var response = await businessLogic.GetUserId(userEmailDto.Email);

            if (response.IsSuccessful)
                return Ok(response.Result);

            return BadRequest(new { message = response.ErrorMessage });
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("get-user-information", Name = "api.post.internal.information")]
        // POST: api/internalcall/getuserinformation
        public async Task<IActionResult> PostInformation(
            [FromBody] UserEmailDto userEmailDto)
        {
            var response = await businessLogic.GetUserInformation(userEmailDto.Email);

            if (response.IsSuccessful)
                return Ok(response.Result);

            return BadRequest(new { message = response.ErrorMessage });
        }
    }
}