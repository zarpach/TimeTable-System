using iuca.Application.ViewModels.Users.Roles;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Roles
{
    public interface IRoleService
    {
        /// <summary>
        /// Get roles grouped by name
        /// </summary>
        /// <returns>List of grouped roles with their ids</returns>
        List<RoleViewModel> GetRoles();

        /// <summary>
        /// Get roles grouped by name
        /// </summary>
        /// <returns>List of grouped roles with their ids</returns>
        List<RoleViewModel> GetRoles(int organizationId);

        /// <summary>
        /// Get role by name prefix
        /// </summary>
        /// <param name="roleNamePrefix">Role name prefix</param>
        /// <returns>Role name with role ids</returns>
        RoleViewModel GetRoleByNamePrefix(string roleNamePrefix);

        /// <summary>
        /// Create role for each organization
        /// </summary>
        /// <param name="roleName">Role name</param>
        IdentityResult Create(string roleName);

        /// <summary>
        /// Edit role for each role id
        /// </summary>
        /// <param name="newRoleName">New role name prefix</param>
        /// <param name="roleIds">List of role ids</param>
        /// <returns>Identity result</returns>
        IdentityResult Edit(string newRoleName, List<string> roleIds);

        /// <summary>
        /// Delete roles by ids
        /// </summary>
        /// <param name="roleIds">Role ids</param>
        /// <returns>Identity result</returns>
        IdentityResult Delete(List<string> roleIds);
    }
}
