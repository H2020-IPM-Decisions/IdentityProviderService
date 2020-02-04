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
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;
        private readonly IDataService dataService;
        private readonly IConfiguration config;

        public TestController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper,
            IDataService dataService,
            IConfiguration config)
        {
            this.dataService = dataService
                ?? throw new System.ArgumentNullException(nameof(dataService));
            this.config = config 
                ?? throw new ArgumentNullException(nameof(config));
            this.userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            this.signInManager = signInManager
                ?? throw new ArgumentNullException(nameof(signInManager));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }      

        [Authorize]
        [HttpGet("protected")]
        public string Protected()
        {
            return "Only if you have a valid token!";
        }
    }
}