using System;
using System.Threading.Tasks;
using System.Web;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {

        public async Task<GenericResponse> ForgotPassword(UserEmailDto userEmailDto)
        {
            try
            {
                var identityUser = await this.dataService.UserManager.FindByEmailAsync(userEmailDto.Email);

                if (identityUser == null)
                    return GenericResponseBuilder.Success();

                var token = await dataService.UserManager.GeneratePasswordResetTokenAsync(identityUser);
                var configKey = "UIPageAddresses:ResetPasswordFormPageAddress";
                var emailObject = GenerateEmailLink(identityUser, configKey, token, "email");
                var passwordEmail = this.mapper.Map<ForgotPasswordEmail>(emailObject);
                var emailSent = await this.emailProvider.SendForgotPasswordEmail(passwordEmail);

                if (!emailSent)
                    return GenericResponseBuilder.NoSuccess("Email send failed");

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ForgotPassword. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess(ex.Message.ToString());
            }
        }

        public async Task<GenericResponse> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var identityUser = await this.dataService.UserManager.FindByEmailAsync(resetPasswordDto.Email);

                if (identityUser == null)
                    return GenericResponseBuilder.Success();

                IdentityResult identityResult = await this.dataService.UserManager.ResetPasswordAsync(
                    identityUser, HttpUtility.UrlDecode(resetPasswordDto.Token), resetPasswordDto.Password);

                if (!identityResult.Succeeded)
                    return GenericResponseBuilder.NoSuccess<IdentityResult>(identityResult);

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ResetPassword. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess(ex.Message.ToString());
            }
        }

        public async Task<GenericResponse> AddNewUser(UserForRegistrationDto user)
        {
            try
            {
                var identityUser = this.mapper.Map<ApplicationUser>(user);
                var identityResult = await this.dataService.UserManager.CreateAsync(identityUser, user.Password);

                if (identityResult.Succeeded)
                {
                    await AddInitialClaim(identityUser, user.UserType);

                    var token = await dataService.UserManager.GenerateEmailConfirmationTokenAsync(identityUser);
                    var configKey = "UIPageAddresses:ConfirmUserFormPageAddress";
                    var emailObject = GenerateEmailLink(identityUser, configKey, token, "id");                    
                    var registrationEmail = this.mapper.Map<RegistrationEmail>(emailObject);
                    var emailSent = await this.emailProvider.SendRegistrationEmail(registrationEmail);
                    
                    var userToReturn = this.mapper.Map<UserRegistrationReturnDto>(identityUser);
                    if (!emailSent)
                        userToReturn.EmailSentDuringRegistration = false;
                                           
                    var successResponse = GenericResponseBuilder.Success<UserDto>(userToReturn);
                    return successResponse;
                }

                var failResponse = GenericResponseBuilder.NoSuccess<IdentityResult>(identityResult);
                return failResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewUser. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess<IdentityResult>(null, ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<AuthenticationDto>> AuthenticateUser(UserForAuthenticationDto user, HttpRequest request)
        {
            try
            {
                if (request.Headers["grant_type"].ToString().ToLower() != "password")
                    return GenericResponseBuilder.NoSuccess<AuthenticationDto>(null, "Wrong grant type");

                var isValidClient = await this.authenticationProvider.ValidateApplicationClientAsync(request);
                if (!isValidClient.IsSuccessful)
                    return GenericResponseBuilder.NoSuccess<AuthenticationDto>(null, isValidClient.ResponseMessage);

                var isAuthorize = await this.authenticationProvider.ValidateUserAuthenticationAsync(user);
                if (!isAuthorize.IsSuccessful)
                    return GenericResponseBuilder.NoSuccess<AuthenticationDto>(null, isAuthorize.ResponseMessage);

                var userClaims = await this.dataService.UserManager.GetClaimsAsync(isAuthorize.Result);
                var userRoles = await this.dataService.UserManager.GetRolesAsync(isAuthorize.Result);

                AuthenticationDto authentificationDto = await CreateAuthentificationDto(isValidClient, isAuthorize);
                return GenericResponseBuilder.Success<AuthenticationDto>(authentificationDto);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AuthenticateUser. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess<AuthenticationDto>(null, ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<AuthenticationDto>> AuthenticateUser(HttpRequest request)
        {
            try
            {
                if (request.Headers["grant_type"].ToString().ToLower() != "refresh_token")
                    return GenericResponseBuilder.NoSuccess<AuthenticationDto>(null, "Wrong grant type");

                var isValidClient = await this.authenticationProvider.ValidateApplicationClientAsync(request);
                if (!isValidClient.IsSuccessful)
                    return GenericResponseBuilder.NoSuccess<AuthenticationDto>(null, isValidClient.ResponseMessage);

                var refreshTokenFromHeader = request.Headers["refresh_token"].ToString();
                var isValidRefreshToken = await this.refreshTokenProvider.ValidateRefreshToken(isValidClient.Result, refreshTokenFromHeader);
                if (!isValidRefreshToken.IsSuccessful)
                    return GenericResponseBuilder.NoSuccess<AuthenticationDto>(null, isValidRefreshToken.ResponseMessage);

                var isAuthorize = await this.authenticationProvider.ValidateUserAsync(isValidRefreshToken.Result.UserId);
                if (!isAuthorize.IsSuccessful)
                    return GenericResponseBuilder.NoSuccess<AuthenticationDto>(null, isAuthorize.ResponseMessage);

                AuthenticationDto authentificationDto = await CreateAuthentificationDto(isValidClient, isAuthorize);
                return GenericResponseBuilder.Success<AuthenticationDto>(authentificationDto);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AuthenticateUser. {0}. {0}. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess<AuthenticationDto>(null, ex.Message.ToString());
            }
        }

        private async Task<AuthenticationDto> CreateAuthentificationDto(AuthenticationProviderResult<ApplicationClient> isValidClient,
            AuthenticationProviderResult<ApplicationUser> isAuthorize)
        {
            var userClaims = await this.authenticationProvider.GetUserClaimsAsync(isAuthorize.Result);
            var userRoles = await this.authenticationProvider.GetUserRolesAsync(isAuthorize.Result);

            var claims = await this.jWTProvider.GetValidClaims(isAuthorize.Result, userRoles, userClaims);
            var token = this.jWTProvider.GenerateToken(claims, isValidClient.Result.JWTAudienceCategory);
            var refreshToken = await this.refreshTokenProvider.GenerateRefreshToken(isAuthorize.Result, isValidClient.Result);

            var bearerToken = new AuthenticationDto()
            {
                Id = Guid.Parse(isAuthorize.Result.Id),
                Email = isAuthorize.Result.Email,
                Roles = userRoles,
                Claims = userClaims,
                Token = token,
                RefreshToken = refreshToken
            };
            return bearerToken;
        }

        private async Task AddInitialClaim(ApplicationUser userEntity, string userType)
        {
            var userTypeClaim = this.configuration["AccessClaims:ClaimTypeName"];
            string userTypeClaimValue = !string.IsNullOrEmpty(userType) ? userType : this.configuration["AccessClaims:DefaultUserAccessLevel"];
            if (userTypeClaim == null | userTypeClaimValue == null)
            {
                throw new Exception("AccessClaims section missing on appsettings.json file");
            }
            await this.dataService.UserManager.AddClaimAsync(userEntity, CreateClaim(userTypeClaim, userTypeClaimValue));
        }

        public async Task<GenericResponse> ConfirmEmail(Guid userId, string token)
        {
            try
            {
                var userToConfirm = await this.dataService.UserManager.FindByIdAsync(userId.ToString());
                if (userToConfirm == null) return GenericResponseBuilder.NoSuccess<IdentityResult>(null, "Not found");

                var identityResult = await this.dataService.UserManager.ConfirmEmailAsync(userToConfirm, token);
                if (identityResult.Succeeded)
                    return GenericResponseBuilder.Success();

                return GenericResponseBuilder.NoSuccess<IdentityResult>(identityResult);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ConfirmEmail. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess<IdentityResult>(null, ex.Message.ToString());
            }
        }

        private Email GenerateEmailLink(IdentityUser identityUser, string configKey, string token, string userInformation)
        {
            var queryString = string.Format("token={0}",HttpUtility.UrlEncode(token));
            switch (userInformation.ToLower())
            {
                case "id":
                    queryString = string.Format("{0}&userId={1}", queryString, identityUser.Id);
                    break;
                case "email":
                    queryString = string.Format("{0}&email={1}", queryString, identityUser.Email);
                    break;                
                default:
                    break;
            }
            var uiAddress = this.configuration[configKey];
            var link = new Uri(string.Format("{0}?{1}", uiAddress, queryString));

            var email = new Email(){
                CallbackUrl = link,
                Token = token,
                ToAddress = identityUser.Email
            };
            return email;
        }
    }
}