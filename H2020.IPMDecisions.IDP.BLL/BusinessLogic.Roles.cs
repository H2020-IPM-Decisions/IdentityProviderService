using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using H2020.IPMDecisions.IDP.Core.Helpers;
using System.Linq;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse> CreateRole(RoleForManipulationDto role, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                    out MediaTypeHeaderValue parsedMediaType))
                {
                    return GenericResponseBuilder.NoSuccess("Wrong media type");
                }
                var roleEntity = this.mapper.Map<IdentityRole>(role);
                var result = await this.dataService.RoleManager.CreateAsync(roleEntity);

                if (!result.Succeeded) return GenericResponseBuilder.NoSuccess<IdentityResult>(result);

                var roleToReturn = this.mapper.Map<RoleDto>(roleEntity)
                    .ShapeData()
                    as IDictionary<string, object>; ;

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

                if (includeLinks)
                {
                    var links = CreateLinksForRole(Guid.Parse(roleEntity.Id));
                    roleToReturn.Add("links", links);
                }

                return GenericResponseBuilder.Success<IDictionary<string, object>>(roleToReturn);
            }
            catch (Exception ex)
            {
                //TODO: log error
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, ex.Message.ToString());
            }
        }

        public async Task<GenericResponse> DeleteRole(Guid id)
        {
            try
            {
                var roleEntity = await this.dataService.RoleManager.FindByIdAsync(id.ToString());
                if (roleEntity == null) return GenericResponseBuilder.Success();

                var result = await this.dataService.RoleManager.DeleteAsync(roleEntity);

                if (!result.Succeeded)
                    return GenericResponseBuilder.NoSuccess(result);

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                //TODO: log error
                return GenericResponseBuilder.NoSuccess(ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<IDictionary<string, object>>> GetRole(Guid id, string fields, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                out MediaTypeHeaderValue parsedMediaType))
                {
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type");
                }
                if (!propertyCheckerService.TypeHasProperties<RoleDto>(fields))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong fields entered");

                var roleEntity = await this.dataService.RoleManager.FindByIdAsync(id.ToString());

                if (roleEntity == null)
                {
                    var emptyDictionary = new Dictionary<string, object>();
                    return GenericResponseBuilder.Success<IDictionary<string, object>>(emptyDictionary);
                }

                var roleToReturn = this.mapper.Map<RoleDto>(roleEntity)
                    .ShapeData(fields)
                    as IDictionary<string, object>;

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                                 .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

                if (includeLinks)
                {
                    var links = CreateLinksForRole(id, fields);
                    roleToReturn.Add("links", links);
                }

                return GenericResponseBuilder.Success<IDictionary<string, object>>(roleToReturn);
            }
            catch (Exception ex)
            {
                //TODO: log error
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<IEnumerable<IDictionary<string, object>>>> GetRoles(string fields, string mediaType)
        {         
            /**************************************************************************************************************/          
            //For debugging, to see if we can write to volume in docker
            /*string path = Path.Combine(Directory.GetCurrentDirectory(), "logs","example.txt");
            Console.WriteLine($"PATH {path}");
                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(path, true))
                    {
                        file.WriteLine("Hello CS");
                    }
            */
            //For debugging, test the logger           
            /*NLog.LogManager.ThrowExceptions = true; // TODO Remove this when done trouble-shooting
            NLog.Common.InternalLogger.LogLevel = NLog.LogLevel.Debug;
            NLog.Common.InternalLogger.LogToConsole = true;
            NLog.Common.InternalLogger.LogFile = "/app/logs/NLog.Internal.txt";
            NLog.Logger loggerx = NLog.LogManager.GetLogger("foo");
            loggerx.Info("Program started");
            */
            //NLog.LogManager.Shutdown();  // Remember to flush
            
            //For debugging, test the logger using NLog.config 
            //logger.Error("Hello CS");
            /**************************************************************************************************************/ 

            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                   out MediaTypeHeaderValue parsedMediaType))
                {
                    return GenericResponseBuilder.NoSuccess<IEnumerable<IDictionary<string, object>>>(null, "Wrong media type");
                }

                if (!this.propertyCheckerService.TypeHasProperties<RoleDto>(fields, true))
                {
                    return GenericResponseBuilder.NoSuccess<IEnumerable<IDictionary<string, object>>>(null, "Wrong fields entered");
                }

                var roles = await this.dataService.RoleManager.Roles.ToListAsync();

                var rolesToReturn = this.mapper
                    .Map<IEnumerable<RoleDto>>(roles)
                    .ShapeData(fields);

                if (roles.Count == 0)
                    return GenericResponseBuilder.Success<IEnumerable<IDictionary<string, object>>>(rolesToReturn);

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                   .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

                var rolesToReturnWithLinks = rolesToReturn.Select(role =>
                {
                    var roleAsDictionary = role as IDictionary<string, object>;
                    if (includeLinks)
                    {
                        var rolesLinks = CreateLinksForRole((Guid)roleAsDictionary["Id"], fields);
                        roleAsDictionary.Add("links", rolesLinks);
                    }
                    return roleAsDictionary;
                });

                return GenericResponseBuilder.Success<IEnumerable<IDictionary<string, object>>>(rolesToReturn);
            }
            catch (Exception ex)
            {
                //TODO: log error
                return GenericResponseBuilder.NoSuccess<IEnumerable<IDictionary<string, object>>>(null, ex.Message.ToString());
            }
        }

        #region helpers
        private IEnumerable<LinkDto> CreateLinksForRole(
            Guid id,
            string fields = "")
        {
            var links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(
                    url.Link("GetRole", new { id }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                    url.Link("GetRole", new { id, fields }),
                    "self",
                    "GET"));
            }

            links.Add(new LinkDto(
                url.Link("DeleteRole", new { id }),
                "delete_role",
                "DELETE"));

            return links;
        }
        #endregion
    }
}