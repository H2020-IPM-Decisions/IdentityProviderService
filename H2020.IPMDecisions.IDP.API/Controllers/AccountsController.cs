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
    public class AccountsController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            this.userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            this.mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [AllowAnonymous]
        [HttpPost("Register", Name = "RegisterUser")]
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

        // [AllowAnonymous]
        // [HttpPost("authenticate", Name = "AuthenticateUser")]
        // public async Task<IActionResult> Authenticate([FromBody]UserForAuthentificationDto userDto)
        // {
        //     // var user = _userService.Authenticate(user, user.Password);

        //     // if (user == null)
        //     //     return BadRequest(new { message = "Username or password is incorrect" });

        //     // return Ok(user);
        // }
    }
}