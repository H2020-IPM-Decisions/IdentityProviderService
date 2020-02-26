using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using H2020.IPMDecisions.IDP.Core.Helpers;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using System.Linq;
using System;
using H2020.IPMDecisions.IDP.Core.Services;
using System.Text.Json;
using Microsoft.Net.Http.Headers;
using H2020.IPMDecisions.IDP.Core.Entities;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "Admin", AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("/api/refreshtokens")]
    public class RefreshTokensController : ControllerBase
    {
        private readonly IDataService dataService;
        private readonly IMapper mapper;
        private readonly IPropertyCheckerService propertyCheckerService;

        public RefreshTokensController(
            IDataService dataService,
            IMapper mapper,
            IPropertyCheckerService propertyCheckerService)
        {
            this.dataService = dataService
                ?? throw new System.ArgumentNullException(nameof(dataService));
            this.mapper = mapper
                ?? throw new System.ArgumentNullException(nameof(mapper));
            this.propertyCheckerService = propertyCheckerService
                ?? throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpGet("", Name = "GetRefreshTokens")]
        [HttpHead]
        // GET: api/refreshtokens
        public async Task<IActionResult> Get(
            [FromQuery] RefreshTokenResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
                out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!propertyCheckerService.TypeHasProperties<RefreshToken>(resourceParameter.Fields, true))
                return BadRequest();

            var refreshTokens = await this.dataService.RefreshTokens.FindAllAsync(resourceParameter);
            if (refreshTokens.Count() == 0) return NotFound();

            var paginationMetaData = new
            {
                totalCount = refreshTokens.TotalCount,
                pageSize = refreshTokens.PageSize,
                currentPage = refreshTokens.CurrentPage,
                totalPages = refreshTokens.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetaData));

            var links = CreateLinksForRefreshTokens(resourceParameter, refreshTokens.HasNext, refreshTokens.HasPrevious);

            var shapedRefreshTokens = this.mapper
                .Map<IEnumerable<RefreshTokenDto>>(refreshTokens)
                .ShapeData(resourceParameter.Fields);

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
 
            var refreshTokensToReturnWithLinks = shapedRefreshTokens.Select(token =>
            {
                var tokenAsDictionary = token as IDictionary<string, object>;
                if (includeLinks)
                {
                    var userLinks = CreateLinksForRefreshToken((Guid)tokenAsDictionary["Id"], resourceParameter.Fields);
                    tokenAsDictionary.Add("links", userLinks);
                }
                return tokenAsDictionary;
            });

            var RefreshTokensToReturn = new
            {
                value = refreshTokensToReturnWithLinks,
                links
            };

            return Ok(RefreshTokensToReturn);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpGet("{id:guid}", Name = "GetRefreshToken")]
        // GET: api/applicationclient/1
        public async Task<IActionResult> Get(
            [FromRoute] Guid id,
            [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
                out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!propertyCheckerService.TypeHasProperties<RefreshToken>(fields))
                return BadRequest();

            var refreshTokenFromRepository = await this.dataService.RefreshTokens.FindByIdAsync(id);
            if (refreshTokenFromRepository == null) return NotFound();


            var clientToReturn = this.mapper
                .Map<RefreshTokenDto>(refreshTokenFromRepository)
                .ShapeData(fields)
                as IDictionary<string, object>;

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                            .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            if (includeLinks)
            {
                var links = CreateLinksForRefreshToken(id, fields);
                clientToReturn.Add("links", links);
            }
            return Ok(clientToReturn);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:guid}", Name = "DeleteRefreshToken")]
        //DELETE :  api/refreshtokens/1
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var refreshTokenToDelete = await this.dataService.RefreshTokens.FindByIdAsync(id);

            if (refreshTokenToDelete == null) return NotFound();

            this.dataService.RefreshTokens.Delete(refreshTokenToDelete);
            await this.dataService.CompleteAsync();

            return NoContent();
        }

        #region helpers
        private string CreateRefreshTokenResourceUri(
               RefreshTokenResourceParameter resourceParameters,
               ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetRefreshTokens",
                    new
                    {
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber - 1,
                        pageSize = resourceParameters.PageSize,
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetRefreshTokens",
                    new
                    {
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber + 1,
                        pageSize = resourceParameters.PageSize,
                    });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetRefreshTokens",
                    new
                    {
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber,
                        pageSize = resourceParameters.PageSize,
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForRefreshTokens(
            RefreshTokenResourceParameter resourceParameters,
            bool hasNextPage,
            bool hasPreviousPage)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                CreateRefreshTokenResourceUri(resourceParameters, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNextPage)
            {
                links.Add(new LinkDto(
                CreateRefreshTokenResourceUri(resourceParameters, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }
            if (hasPreviousPage)
            {
                links.Add(new LinkDto(
               CreateRefreshTokenResourceUri(resourceParameters, ResourceUriType.PreviousPage),
               "previous_page",
               "GET"));
            }
            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForRefreshToken(
            Guid id,
            string fields = "")
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(
                Url.Link("GetRefreshToken", new { id }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                 Url.Link("GetRefreshToken", new { id, fields }),
                 "self",
                 "GET"));
            }

            links.Add(new LinkDto(
                Url.Link("DeleteRefreshToken", new { id }),
                "delete_refresh_token",
                "DELETE"));

            return links;
        }
        #endregion

    }
}