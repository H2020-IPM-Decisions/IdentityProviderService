using System.Collections.Generic;
using H2020.IPMDecisions.IDP.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Route("/api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet("", Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(Url.Link("GetRoot", new { }),
                "self",
                "GET"));

            links.Add(
                new LinkDto(Url.Link("RegisterUser", new { }),
                "register",
                "POST"));

            links.Add(
                new LinkDto(Url.Link("AuthenticateUser", new { }),
                "authenticate",
                "POST"));

            return Ok(links);
        }
    }
}