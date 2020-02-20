using System;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Entities;
using H2020.IPMDecisions.IDP.Core.Helpers;
using H2020.IPMDecisions.IDP.Core.ResourceParameters;
using H2020.IPMDecisions.IDP.Core.Services;
using H2020.IPMDecisions.IDP.Data.Core.Repositories;
using Microsoft.AspNetCore.Identity;

namespace H2020.IPMDecisions.IDP.Data.Persistence.Repositories
{
    public class UserManagerExtensionRepository: IUserManagerExtensionRepository
    {
        private readonly UserManager<ApplicationUser> userManager;
        private IPropertyMappingService propertyMappingService;

        public UserManagerExtensionRepository(
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public UserManagerExtensionRepository(UserManager<ApplicationUser> userManager, IPropertyMappingService propertyMappingService) : this(userManager)
        {
            this.propertyMappingService = propertyMappingService;
        }

        public async Task<PagedList<ApplicationUser>> FindAllAsync(ApplicationUserResourceParameter resourceParameter)
        {
            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));
            var collection = this.userManager.Users as IQueryable<ApplicationUser>;

            if (!string.IsNullOrEmpty(resourceParameter.SearchQuery))
            {
                var searchQuery = resourceParameter.SearchQuery.Trim();
                collection = collection.Where(u =>
                    u.UserName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                    || u.Email.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(resourceParameter.OrderBy))
            {
                var propertyMappingDictionary =
                    this.propertyMappingService.GetPropertyMapping<UserDto, ApplicationUser>();

                collection = collection.ApplySort(resourceParameter.OrderBy, propertyMappingDictionary);

            }

            return await PagedList<ApplicationUser>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }
    }
}