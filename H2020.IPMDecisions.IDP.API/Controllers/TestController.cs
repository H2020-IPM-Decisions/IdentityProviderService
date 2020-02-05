using System;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.Extensions.Configuration;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("/api/test")]
    public class TestController : ControllerBase
    {

        public TestController()
        {
        }

        [HttpGet("notprotected")]
        public string NotProtected()
        {
            return "No token needed!";
        }

        [Authorize]
        [HttpGet("protected")]
        public string Protected()
        {
            return "Only if you have a valid token!";
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("SuperAdmin")]
        public string ProtectedSuperAdmin()
        {
            return "Only if you have a valid token!";
        }

        [Authorize(Roles = "ExtraRole")]
        [HttpGet("ExtraRole")]
        public string ProtectedExtraRole()
        {
            return "Only if you have a valid token!";
        }

        [Authorize(Roles = "SuperAdmin, ExtraRole")]
        [HttpGet("BothRoles")]
        public string ProtectedBothRoles()
        {
            return "Only if you have a valid token!";
        }
    }
}