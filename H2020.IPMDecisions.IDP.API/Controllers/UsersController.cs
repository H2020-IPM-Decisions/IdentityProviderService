using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Helpers;
using H2020.IPMDecisions.IDP.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using System.Text.Json;
using H2020.IPMDecisions.IDP.Data.Core;
using H2020.IPMDecisions.IDP.Core.Services;
using H2020.IPMDecisions.IDP.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Mime;
using Microsoft.Net.Http.Headers;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin", AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IDataService dataService;
        private readonly IPropertyMappingService propertyMappingService;
        private readonly IPropertyCheckerService propertyCheckerService;

        public UsersController(
            IMapper mapper,
            IDataService dataService,
            IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
            this.propertyMappingService = propertyMappingService
                ?? throw new ArgumentNullException(nameof(propertyMappingService));
            this.propertyCheckerService = propertyCheckerService
                ?? throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpGet("", Name = "GetUsers")]
        [HttpHead]
        // GET: api/users
        public async Task<IActionResult> GetUsers(
            [FromQuery] ApplicationUserResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
                out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!propertyMappingService.ValidMappingExistsFor<UserDto, ApplicationUser>(resourceParameter.OrderBy))
                return BadRequest();
            if (!propertyCheckerService.TypeHasProperties<UserDto>(resourceParameter.Fields, true))
                return BadRequest();           

            var users = await this.dataService.UserManagerExtensions.FindAllAsync(resourceParameter);
            if (users.Count == 0) return NotFound();

            var paginationMetaData = new
            {
                totalCount = users.TotalCount,
                pageSize = users.PageSize,
                currentPage = users.CurrentPage,
                totalPages = users.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetaData));

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

            var usersToReturn = new
            {
                value = shapedUsersToReturnWithLinks,
                links
            };

            return Ok(usersToReturn);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpGet("{id:guid}", Name = "GetUser")]
        // GET: api/users/1
        public async Task<IActionResult> GetUser(
            [FromRoute] Guid id,
            [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
                out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }
            if (!propertyCheckerService.TypeHasProperties<UserDto>(fields))
                return BadRequest();

            var user = await this.dataService.UserManager.FindByIdAsync(id.ToString());

            if (user == null) return NotFound();

            var userToReturn = this.mapper.Map<UserDto>(user)
                .ShapeData(fields)
                as IDictionary<string, object>;

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            if (includeLinks)
            {
                var links = CreateLinksForUser(id, fields);
                userToReturn.Add("links", links);
            }

            return Ok(userToReturn);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:guid}", Name = "DeleteUser")]
        // DELETE: api/users/1
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            var userToDelete = await this.dataService.UserManager.FindByIdAsync(id.ToString());

            if (userToDelete == null) return NotFound();

            var result = await this.dataService.UserManager.DeleteAsync(userToDelete);

            if (result.Succeeded) return NoContent();

            return BadRequest(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        // OPTIONS: api/users
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST,GET,DELETE");
            return Ok();
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
                Url.Link("GetUser", new { id }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                Url.Link("GetUser", new { id, fields }),
                "self",
                "GET"));
            }

            links.Add(new LinkDto(
                Url.Link("DeleteUser", new { id }),
                "delete_user",
                "DELETE"));

            links.Add(new LinkDto(
                Url.Link("GetRolesFromUser", new { userId = id }),
                "roles",
                "GET"));

            links.Add(new LinkDto(
                Url.Link("AssignRolesToUser", new { userId = id }),
                "assign_roles_to_user",
                "POST"));

            links.Add(new LinkDto(
                Url.Link("RemoveRolesFromUser", new { userId = id }),
                "remove_roles_from_user",
                "DELETE"));

            links.Add(new LinkDto(
                Url.Link("GetClaimsFromUser", new { userId = id }),
                "claims",
                "GET"));

            links.Add(new LinkDto(
                Url.Link("AssignClaimsToUser", new { userId = id }),
                "assign_claims_to_user",
                "POST"));

            links.Add(new LinkDto(
                Url.Link("RemoveClaimsFromUser", new { userId = id }),
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
                    return Url.Link("GetUsers",
                    new
                    {
                        fields = resourceParameters.Fields,
                        orderBy = resourceParameters.OrderBy,
                        pageNumber = resourceParameters.PageNumber - 1,
                        pageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetUsers",
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
                    return Url.Link("GetUsers",
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