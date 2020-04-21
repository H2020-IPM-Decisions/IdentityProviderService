using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Helpers;
using H2020.IPMDecisions.IDP.Core.Models;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using Microsoft.Net.Http.Headers;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse> DeleteRefreshToken(Guid id)
        {
            try
            {
                var refreshTokenToDelete = await this.dataService.RefreshTokens.FindByIdAsync(id);
                if (refreshTokenToDelete == null) return GenericResponseBuilder.Success();

                this.dataService.RefreshTokens.Delete(refreshTokenToDelete);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                //TODO: log error
                return GenericResponseBuilder.NoSuccess(ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<IDictionary<string, object>>> GetRefreshToken(Guid id, string fields, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                    out MediaTypeHeaderValue parsedMediaType))
                {
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type");
                }

                if (!propertyCheckerService.TypeHasProperties<RefreshToken>(fields))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong fields entered");

                var refreshTokenFromRepository = await this.dataService.RefreshTokens.FindByIdAsync(id);
                if (refreshTokenFromRepository == null) return GenericResponseBuilder.Success<IDictionary<string, object>>(null);

                var tokenToReturn = this.mapper
                    .Map<RefreshTokenDto>(refreshTokenFromRepository)
                    .ShapeData(fields)
                    as IDictionary<string, object>;

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                    .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                if (includeLinks)
                {
                    var links = CreateLinksForRefreshToken(id, fields);
                    tokenToReturn.Add("links", links);
                }
                return GenericResponseBuilder.Success<IDictionary<string, object>>(tokenToReturn);
            }
            catch (Exception ex)
            {
                //TODO: log error
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<ShapedDataWithLinks>> GetRefreshTokens(RefreshTokenResourceParameter resourceParameter, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                   out MediaTypeHeaderValue parsedMediaType))
                {
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong media type");
                }

                var refreshTokens = await this.dataService.RefreshTokens.FindAllAsync(resourceParameter);
                if (refreshTokens.Count == 0) return GenericResponseBuilder.Success<ShapedDataWithLinks>(null);

                var paginationMetaData = new PaginationMetaData
                {
                    TotalCount = refreshTokens.TotalCount,
                    PageSize = refreshTokens.PageSize,
                    CurrentPage = refreshTokens.CurrentPage,
                    TotalPages = refreshTokens.TotalPages
                };

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

                var refreshTokensToReturn = new ShapedDataWithLinks()
                {
                    Value = refreshTokensToReturnWithLinks,
                    Links = links,
                    PaginationMetaData = paginationMetaData
                };

                return GenericResponseBuilder.Success<ShapedDataWithLinks>(refreshTokensToReturn);
            }
            catch (Exception ex)
            {
                //TODO: log error
                return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, ex.Message.ToString());
            }
        }

        #region helpers
        private string CreateRefreshTokenResourceUri(
               RefreshTokenResourceParameter resourceParameters,
               ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return url.Link("GetRefreshTokens",
                    new
                    {
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber - 1,
                        pageSize = resourceParameters.PageSize,
                    });
                case ResourceUriType.NextPage:
                    return url.Link("GetRefreshTokens",
                    new
                    {
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber + 1,
                        pageSize = resourceParameters.PageSize,
                    });
                case ResourceUriType.Current:
                default:
                    return url.Link("GetRefreshTokens",
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
                url.Link("GetRefreshToken", new { id }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                 url.Link("GetRefreshToken", new { id, fields }),
                 "self",
                 "GET"));
            }

            links.Add(new LinkDto(
                url.Link("DeleteRefreshToken", new { id }),
                "delete_refresh_token",
                "DELETE"));

            return links;
        }
        #endregion
    }
}