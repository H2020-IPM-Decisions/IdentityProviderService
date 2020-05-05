using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Core.Dtos;
using H2020.IPMDecisions.IDP.Core.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace H2020.IPMDecisions.IDP.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IList<string>>> GetUserRoles(Guid id)
        {
            try
            {
                var user = await this.dataService.UserManager.FindByIdAsync(id.ToString());
                if (user == null) return GenericResponseBuilder.Success<IList<string>>(null);

                var rolesToReturn = await this.dataService.UserManager.GetRolesAsync(user);
                if (rolesToReturn.Count == 0) return GenericResponseBuilder.Success<IList<string>>(null);

                return GenericResponseBuilder.Success<IList<string>>(rolesToReturn);
            }
            catch (Exception ex)
            {
                //TODO: log error
                return GenericResponseBuilder.NoSuccess<IList<string>>(null, ex.Message.ToString());
            }
        }

        public async Task<GenericResponse<UserDto>> ManageUserRoles(Guid id, List<RoleForManipulationDto> roles, bool remove = false)
        {
            try
            {
                var user = await this.dataService.UserManager.FindByIdAsync(id.ToString());
                if (user == null) return GenericResponseBuilder.Success<UserDto>(null);

                var currentUserRoles = await this.dataService.UserManager.GetRolesAsync(user);

                foreach (var role in roles)
                {
                    if (currentUserRoles.Any(r => r.Equals(role.Name, StringComparison.OrdinalIgnoreCase)) & remove)
                    {
                        await this.dataService.UserManager.RemoveFromRoleAsync(user, role.Name);
                    }
                    else if (!currentUserRoles.Any(r => r.Equals(role.Name, StringComparison.OrdinalIgnoreCase)) & !remove)
                    {
                        var roleEntity = await this.dataService.RoleManager.FindByNameAsync(role.Name);
                        if (roleEntity == null)
                        {
                            roleEntity = this.mapper.Map<IdentityRole>(role);
                            await this.dataService.RoleManager.CreateAsync(roleEntity);
                        }
                        await this.dataService.UserManager.AddToRoleAsync(user, role.Name);
                    }
                }
                return GenericResponseBuilder.Success<UserDto>(this.mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                //TODO: log error
                return GenericResponseBuilder.NoSuccess<UserDto>(null, ex.Message.ToString());
            }
        }
    }
}