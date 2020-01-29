using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class Users : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public Users(
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            this.mapper = mapper 
                ?? throw new ArgumentNullException(nameof(mapper));
            this.userManager = userManager
                ?? throw new System.ArgumentNullException(nameof(userManager));
        }

        [HttpGet(Name = "GetUsers")]
        [HttpHead]
        [Route("")]
        // GET: api/users
        public async Task<IActionResult> GetUsers()
        {
            var users = await this.userManager.Users.ToListAsync();
            if (users.Count() == 0) return NotFound();

            var userToReturn = this.mapper.Map<List<UserDto>>(users);

            return Ok(userToReturn);
        }
    }
}