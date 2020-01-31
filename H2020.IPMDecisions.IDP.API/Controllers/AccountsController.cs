using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper)
        {
            this.userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            this.signInManager = signInManager
                ?? throw new ArgumentNullException(nameof(signInManager));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
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

        [AllowAnonymous]
        [HttpPost("Authenticate", Name = "AuthenticateUser")]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthentificationDto userDto)
        {
            var user = await this.userManager.FindByNameAsync(userDto.Username);

            if (user == null) return BadRequest(new { message = "Username or password is incorrect" });

            // ToDo When Email confirmation available
            //if (!user.EmailConfirmed) return BadRequest(new { message = "Email not confirmed" });

            var result = await this.signInManager.PasswordSignInAsync(user.UserName, userDto.Password, false, true);

            if (result.Succeeded)
            {
                var userToReturn = this.mapper.Map<UserDto>(user);
                // ToDo generate JWT here and return
                return Ok(userToReturn);
            }
            else if (result.IsLockedOut)
            {
                return BadRequest(new { message = "Username lockout" });
            }
            else
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
        }
    }
}