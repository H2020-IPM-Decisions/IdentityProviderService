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
    [Route("api/accounts")]
    public class Accounts : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public Accounts(
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

                var userToReturn = this.mapper.Map<UserDto>(userEntity);

                return CreatedAtRoute("GetUserById",
                new { userId = userToReturn.Id },
                userToReturn);
            }

            return BadRequest(result);
        }
    }
}