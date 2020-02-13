using System.Net.Mime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "SuperAdmin", AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("/api/refreshtokens")]
    public class RefreshTokensController : ControllerBase
    {
        public RefreshTokensController()
        {
        }
    }
}