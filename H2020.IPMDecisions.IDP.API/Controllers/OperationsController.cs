using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using H2020.IPMDecisions.IDP.BLL;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/operations")]
    [Authorize(Roles = "Admin", AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    public class OperationsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public OperationsController(
            IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("userreports", Name = "api.operations.post.userreports")]
        // POST: api/operations/userreports
        public IActionResult PostUserReports()
        {
            var response = this.businessLogic.GenerateUserReports();

            if (!response)
                return BadRequest();
            return Accepted();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        // OPTIONS: api/users
        public IActionResult Options()
        {
            Response.Headers.Append("Allow", "OPTIONS,POST,GET,DELETE");
            return Ok();
        }
    }
}