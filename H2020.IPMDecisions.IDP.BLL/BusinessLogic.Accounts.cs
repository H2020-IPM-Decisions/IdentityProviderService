using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse> AddNewUser(UserForRegistrationDto user)
        {
            try
            { 
                var userEntity = this.mapper.Map<ApplicationUser>(user);
                var identityResult = await this.dataService.UserManager.CreateAsync(userEntity, user.Password);

                if (identityResult.Succeeded)
                {
                    await AddInitialClaim(userEntity, user.UserType);

                    //ToDo Generate Email token and return
                    var userToReturn = this.mapper.Map<UserDto>(userEntity);
                    var successResponse = GenericResponseBuilder.Success<UserDto>(userToReturn);
                    return successResponse;
                }

                var failResponse = GenericResponseBuilder.NoSuccess<IdentityResult>(identityResult);
                return failResponse;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, "Error is BLL AddNewUser");
                return GenericResponseBuilder.NoSuccess<AuthenticationDto>(null, ex.Message.ToString());
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
                logger.Error(ex.Message, "Error is BLL AuthenticateUser");
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
                logger.Error(ex.Message, "Error is BLL AuthenticateUser");
                return GenericResponseBuilder.NoSuccess<AuthenticationDto>(null, ex.Message.ToString());
            }
        }

        private async Task<AuthenticationDto> CreateAuthentificationDto(AuthenticationProviderResult<ApplicationClient> isValidClient,
        AuthenticationProviderResult<ApplicationUser> isAuthorize)
        {
            var userClaims = await this.authenticationProvider.GetUserClaimsAsync(isAuthorize.Result);
            var userRoles = await this.authenticationProvider.GetUserRolesAsync(isAuthorize.Result);

            var claims = await this.jWTProvider.GetValidClaims(isAuthorize.Result, userRoles, userClaims);
            var token = this.jWTProvider.GenerateToken(claims, isValidClient.Result.Url);
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
            await this.dataService.UserManager.AddClaimAsync(userEntity, CreateClaim(userTypeClaim, userTypeClaimValue));
        }
    }
}