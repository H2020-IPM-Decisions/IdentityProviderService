using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Helpers;
using H2020.IPMDecisions.IDP.Core.Models;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse> DeleteUser(Guid id)
        {
            try
            {
                var userToDelete = await this.dataService.UserManager.FindByIdAsync(id.ToString());
                if (userToDelete == null) return GenericResponseBuilder.Success();

                var result = await this.dataService.UserManager.DeleteAsync(userToDelete);
                if (!result.Succeeded) return GenericResponseBuilder.NoSuccess(result);

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteUser. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess(ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<IDictionary<string, object>>> GetUser(Guid id, string fields, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                    out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type");

                if (!propertyCheckerService.TypeHasProperties<UserWithRolesClaimsDto>(fields))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong fields entered");

                var user = await this.dataService.UserManager.FindByIdAsync(id.ToString());
                if (user == null) return GenericResponseBuilder.Success<IDictionary<string, object>>(null);

                var userToReturn = this.mapper.Map<UserWithRolesClaimsDto>(user);
                userToReturn.Roles = await this.dataService.UserManager.GetRolesAsync(user);
                userToReturn.Claims = await this.dataService.UserManager.GetClaimsAsync(user);

                var userToReturnShaped = userToReturn
                    .ShapeData(fields)
                    as IDictionary<string, object>;

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                    .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

                if (includeLinks)
                {
                    var links = CreateLinksForUser(id, fields);
                    userToReturnShaped.Add("links", links);
                }

                return GenericResponseBuilder.Success<IDictionary<string, object>>(userToReturnShaped);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetUser. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<ShapedDataWithLinks>> GetUsers(ApplicationUserResourceParameter resourceParameter, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                    out MediaTypeHeaderValue parsedMediaType))
                {
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong media type");
                }

                if (!propertyMappingService.ValidMappingExistsFor<UserDto, ApplicationUser>(resourceParameter.OrderBy))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong OrderBy entered");
                if (!propertyCheckerService.TypeHasProperties<UserDto>(resourceParameter.Fields, true))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong fields entered");

                var users = await this.dataService.UserManagerExtensions.FindAllAsync(resourceParameter);
                if (users.Count == 0) return GenericResponseBuilder.Success<ShapedDataWithLinks>(null);

                var paginationMetaData = new PaginationMetaData
                {
                    TotalCount = users.TotalCount,
                    PageSize = users.PageSize,
                    CurrentPage = users.CurrentPage,
                    TotalPages = users.TotalPages
                };
                var links = CreateLinksForUsers(resourceParameter, users.HasNext, users.HasPrevious);

                var shapedUsersToReturn = this.mapper
                    .Map<IEnumerable<UserDto>>(users)
                    .ShapeData(resourceParameter.Fields);

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                    .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                var shapedUsersToReturnWithLinks = shapedUsersToReturn.Select(user =>
                {
                    var userAsDictionary = user as IDictionary<string, object>;
                    if (includeLinks)
                    {
                        var userLinks = CreateLinksForUser((Guid)userAsDictionary["Id"], resourceParameter.Fields);
                        userAsDictionary.Add("links", userLinks);
                    }
                    return userAsDictionary;
                });

                var usersToReturn = new ShapedDataWithLinks()
                {
                    Value = shapedUsersToReturnWithLinks,
                    Links = links,
                    PaginationMetaData = paginationMetaData
                };

                return GenericResponseBuilder.Success<ShapedDataWithLinks>(usersToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetUsers. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<string>> GetUserId(string userEmail)
        {
            try
            {
                var user = await this.dataService.UserManager.FindByEmailAsync(userEmail);
                if (user == null) return GenericResponseBuilder.NoSuccess<string>(null, "Not found");

                return GenericResponseBuilder.Success(user.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetUserId. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess<string>(null, ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<UserInternalCallDto>> GetUserInformation(string userEmail)
        {
            try
            {
                var user = await this.dataService.UserManager.FindByEmailAsync(userEmail);
                if (user == null) return GenericResponseBuilder.NoSuccess<UserInternalCallDto>(null, "Not found");

                var userToReturn = this.mapper.Map<UserInternalCallDto>(user);
                userToReturn.Claims = await this.dataService.UserManager.GetClaimsAsync(user);

                return GenericResponseBuilder.Success(userToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetUserInformation. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess<UserInternalCallDto>(null, ex.Message.ToString());
            }
        }


        public async Task<GenericResponse> UpdateUser(Guid id, UserForPartialUpdateDto userForUpdate)
        {
            try
            {
                var user = await this.dataService.UserManager.FindByIdAsync(id.ToString());
                if (user == null) return GenericResponseBuilder.NoSuccess("Not found");

                // Default is true
                user.EmailConfirmed = userForUpdate.EmailConfirmed;
                if (!string.IsNullOrEmpty(userForUpdate.Password))
                {
                    var removePasswordResult = await this.dataService.UserManager.RemovePasswordAsync(user);
                    if (!removePasswordResult.Succeeded) return GenericResponseBuilder.NoSuccess(string.Format("Error removing password: {0}: ", removePasswordResult.Errors.FirstOrDefault().Description));
                    var addPasswordResult = await this.dataService.UserManager.AddPasswordAsync(user, userForUpdate.Password);
                    if (!addPasswordResult.Succeeded) return GenericResponseBuilder.NoSuccess(string.Format("Error adding password: {0}", addPasswordResult.Errors.FirstOrDefault().Description));
                }
                await this.dataService.CompleteAsync();
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteUser. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess(ex.Message.ToString());
            }
        }
        #region Helpers
        private IEnumerable<LinkDto> CreateLinksForUser(
            Guid id,
            string fields = "")
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(
                url.Link("GetUser", new { id }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                url.Link("GetUser", new { id, fields }),
                "self",
                "GET"));
            }

            links.Add(new LinkDto(
                url.Link("DeleteUser", new { id }),
                "delete_user",
                "DELETE"));

            links.Add(new LinkDto(
                url.Link("GetRolesFromUser", new { userId = id }),
                "roles",
                "GET"));

            links.Add(new LinkDto(
                url.Link("AssignRolesToUser", new { userId = id }),
                "assign_roles_to_user",
                "POST"));

            links.Add(new LinkDto(
                url.Link("RemoveRolesFromUser", new { userId = id }),
                "remove_roles_from_user",
                "DELETE"));

            links.Add(new LinkDto(
                url.Link("GetClaimsFromUser", new { userId = id }),
                "claims",
                "GET"));

            links.Add(new LinkDto(
                url.Link("AssignClaimsToUser", new { userId = id }),
                "assign_claims_to_user",
                "POST"));

            links.Add(new LinkDto(
                url.Link("RemoveClaimsFromUser", new { userId = id }),
                "remove_claims_from_user",
                "DELETE"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForUsers(
            ApplicationUserResourceParameter resourceParameters,
            bool hasNextPage,
            bool hasPreviousPage)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                CreateUsersResourceUri(resourceParameters, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNextPage)
            {
                links.Add(new LinkDto(
                CreateUsersResourceUri(resourceParameters, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }
            if (hasPreviousPage)
            {
                links.Add(new LinkDto(
                CreateUsersResourceUri(resourceParameters, ResourceUriType.PreviousPage),
                "previous_page",
                "GET"));
            }
            return links;
        }

        private string CreateUsersResourceUri(
               ApplicationUserResourceParameter resourceParameters,
               ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return url.Link("GetUsers",
                    new
                    {
                        fields = resourceParameters.Fields,
                        orderBy = resourceParameters.OrderBy,
                        pageNumber = resourceParameters.PageNumber - 1,
                        pageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery
                    });
                case ResourceUriType.NextPage:
                    return url.Link("GetUsers",
                    new
                    {
                        fields = resourceParameters.Fields,
                        orderBy = resourceParameters.OrderBy,
                        pageNumber = resourceParameters.PageNumber + 1,
                        pageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery
                    });
                case ResourceUriType.Current:
                default:
                    return url.Link("GetUsers",
                    new
                    {
                        fields = resourceParameters.Fields,
                        orderBy = resourceParameters.OrderBy,
                        pageNumber = resourceParameters.PageNumber,
                        pageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery
                    });
            }
        }
        #endregion
    }
}