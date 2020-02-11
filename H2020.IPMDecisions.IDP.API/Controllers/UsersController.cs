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

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "SuperAdmin")]
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

        [HttpGet("", Name = "GetUsers")]
        [HttpHead]
        // GET: api/users
        public async Task<IActionResult> GetUsers([FromQuery] ApplicationUserResourceParameter resourceParameter)
        {
            if (!propertyMappingService.ValidMappingExistsFor<UserDto, ApplicationUser>(resourceParameter.OrderBy))
            {
                return BadRequest();
            }
            if (!propertyCheckerService.TypeHasProperties<UserDto>(resourceParameter.Fields, true))
            {
                return BadRequest();
            }
            
            var users = await this.dataService.UserManagerExtensions.FindAllAsync(resourceParameter);
            if (users.Count == 0) return NotFound();

            var previousPageLink = users.HasPrevious ?
                CreateUsersResourceUri(resourceParameter,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = users.HasNext ?
                CreateUsersResourceUri(resourceParameter,
                ResourceUriType.NextPage) : null;

            var paginationMetaData = new
            {
                totalCount = users.TotalCount,
                pageSize = users.PageSize,
                currentPage = users.CurrentPage,
                totalPages = users.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetaData));

            var usersToReturn = this.mapper
                .Map<IEnumerable<UserDto>>(users)
                .ShapeData(resourceParameter.Fields);

            var usersToReturnWithLinks = usersToReturn.Select(user =>
            {
                var userAsDictionary = user as IDictionary<string, object>;
                var userLinks = CreateLinksForUser((Guid)userAsDictionary["Id"]);
                userAsDictionary.Add("links", userLinks);
                return userAsDictionary;
            });

            return Ok(usersToReturnWithLinks);
        }       

        [HttpGet("{userId:guid}", Name = "GetUser")]
        // GET: api/users/1
        public async Task<IActionResult> GetUser([FromRoute] Guid userId, [FromQuery] string fields)
        {
            if (!propertyCheckerService.TypeHasProperties<UserDto>(fields))
            {
                return BadRequest();
            }
            var user = await this.dataService.UserManager.FindByIdAsync(userId.ToString());

            if (user == null) return NotFound();

            var links = CreateLinksForUser(userId);

            var userToReturn = this.mapper.Map<UserDto>(user)
                .ShapeData(fields)
                as IDictionary<string, object>;

            userToReturn.Add("links", links);

            return Ok(userToReturn);
        }

        [HttpDelete("{userId:guid}", Name = "DeleteUser")]
        // DELETE: api/users/1
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
        {
            var userToDelete = await this.dataService.UserManager.FindByIdAsync(userId.ToString());

            if (userToDelete == null) return NotFound();

            var result = await this.dataService.UserManager.DeleteAsync(userToDelete);

            if (result.Succeeded) return NoContent();

            return BadRequest(result);
        }

        [HttpOptions]
        // OPTIONS: api/users
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST,GET,DELETE");
            return Ok();
        }


        #region Helpers
        private IEnumerable<LinkDto> CreateLinksForUser(
            Guid userId)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                Url.Link("GetUser", new { userId }),
                "self",
                "GET"));

            links.Add(new LinkDto(
                Url.Link("DeleteUser", new { userId }),
                "delete_user",
                "DELETE"));

            links.Add(new LinkDto(
                Url.Link("GetRolesFromUser", new { userId }),
                "roles",
                "GET"));

            links.Add(new LinkDto(
                Url.Link("AssignRolesToUser", new { userId }),
                "assign_roles_to_user",
                "POST"));

            links.Add(new LinkDto(
                Url.Link("RemoveRolesFromUser", new { userId }),
                "remove_roles_to_user",
                "DELETE"));

            links.Add(new LinkDto(
                Url.Link("GetClaimsFromUser", new { userId }),
                "claims",
                "GET"));

            links.Add(new LinkDto(
                Url.Link("AssignClaimsToUser", new { userId }),
                "assign_claims_to_user",
                "POST"));

            links.Add(new LinkDto(
                Url.Link("RemoveClaimsFromUser", new { userId }),
                "remove_claims_to_user",
                "DELETE"));

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