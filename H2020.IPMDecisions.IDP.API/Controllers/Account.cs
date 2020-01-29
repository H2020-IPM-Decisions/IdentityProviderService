using System;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class Account : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public Account(
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            this.userManager = userManager 
                ?? throw new ArgumentNullException(nameof(userManager));
            this.mapper = mapper ?? 
                throw new ArgumentNullException(nameof(mapper));
        }      
        
        [AllowAnonymous]
        [HttpPost(Name ="RegisterUser")]
        [Route("Register")]
        // POST: api/Accounts/Register
        public async Task<ActionResult<UserDto>> Register([FromBody] UserForRegistrationDto userForRegistration)
        {
            var userEntity = this.mapper.Map<ApplicationUser>(userForRegistration);

            var result = await this.userManager.CreateAsync(userEntity, userForRegistration.Password);

            if (result.Succeeded)
            {
                //ToDo Generate Email token and return

                var authorToReturn = this.mapper.Map<UserDto>(userEntity);
                return Ok(authorToReturn);
            }

            return BadRequest();
        }
    }
}