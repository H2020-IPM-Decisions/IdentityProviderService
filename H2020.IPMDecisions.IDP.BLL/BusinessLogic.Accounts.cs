﻿using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse> AddNewUser(UserForRegistrationDto user)
        {
            var userEntity = this.mapper.Map<ApplicationUser>(user);
            var identityResult = await this.dataService.UserManager.CreateAsync(userEntity, user.Password);

            if (identityResult.Succeeded)
            {
                //ToDo Generate Email token and return
                var userToReturn = this.mapper.Map<UserDto>(userEntity);
                var successResponse = GenericResponseBuilder.Success<UserDto>(userToReturn);                
                return successResponse;
            }
            
            var failResponse = GenericResponseBuilder.NoSuccess<IdentityResult>(identityResult);
            return failResponse;
        }

        public async Task<GenericResponse<BearerToken>> AuthenticateUser(UserForAuthenticationDto user, HttpRequest request)
        {
            if (request.Headers["grant_type"].ToString().ToLower() != "password") 
                return GenericResponseBuilder.NoSuccess<BearerToken>(null, "Wrong grant type");

            var isValidClient = await this.authenticationProvider.ValidateApplicationClientAsync(request);
            if (!isValidClient.IsSuccessful)
                return GenericResponseBuilder.NoSuccess<BearerToken>(null, isValidClient.ResponseMessage);
            
            var isAuthorize = await this.authenticationProvider.ValidateUserAuthenticationAsync(user);
            if (!isAuthorize.IsSuccessful)
                return GenericResponseBuilder.NoSuccess<BearerToken>(null, isAuthorize.ResponseMessage);

            BearerToken bearerToken = await CreateBearerToken(isValidClient, isAuthorize);            
            return GenericResponseBuilder.Success<BearerToken>(bearerToken);
        }

        public async Task<GenericResponse<BearerToken>> AuthenticateUser(HttpRequest request)
        {
            if (request.Headers["grant_type"].ToString().ToLower() != "refresh_token")
                return GenericResponseBuilder.NoSuccess<BearerToken>(null, "Wrong grant type");

            var isValidClient = await this.authenticationProvider.ValidateApplicationClientAsync(request);
            if (!isValidClient.IsSuccessful)
                return GenericResponseBuilder.NoSuccess<BearerToken>(null, isValidClient.ResponseMessage);

            var refreshTokenFromHeader = request.Headers["refresh_token"].ToString();
            var isValidRefreshToken = await this.refreshTokenProvider.ValidateRefreshToken(isValidClient.Result, refreshTokenFromHeader);
            if (!isValidRefreshToken.IsSuccessful)
                return GenericResponseBuilder.NoSuccess<BearerToken>(null, isValidRefreshToken.ResponseMessage);

            var isAuthorize = await this.authenticationProvider.FindUserAsync(isValidRefreshToken.Result.UserId);
            if (!isAuthorize.IsSuccessful)
                return GenericResponseBuilder.NoSuccess<BearerToken>(null, isAuthorize.ResponseMessage);
            
            BearerToken bearerToken = await CreateBearerToken(isValidClient, isAuthorize);            
            return GenericResponseBuilder.Success<BearerToken>(bearerToken);
        }

        private async Task<BearerToken> CreateBearerToken(AuthenticationProviderResult<ApplicationClient> isValidClient, 
        AuthenticationProviderResult<ApplicationUser> isAuthorize)
        {
            var claims = await this.jWTProvider.GetValidClaims(isAuthorize.Result);
            var token = this.jWTProvider.GenerateToken(claims, isValidClient.Result.Url);
            var refreshToken = await this.refreshTokenProvider.GenerateRefreshToken(isAuthorize.Result, isValidClient.Result);

            var bearerToken = new BearerToken()
            {
                Token = token,
                RefreshToken = refreshToken
            };
            return bearerToken;
        }
    }
}