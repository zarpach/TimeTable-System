using iuca.Application.ViewModels.Users.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Roles
{
    public interface IPermissionService
    {
        /// <summary>
        /// Get permissions by role name prefix
        /// </summary>
        /// <param name="roleNamePrefix">Role name prefix</param>
        /// <returns>Permissions model</returns>
        PermissionViewModel GetPermissionsByRole(string roleNamePrefix);

        /// <summary>
        /// Update role premission claims
        /// </summary>
        /// <param name="model">Permissions model</param>
        void UpdatePermissions(PermissionViewModel model);
    }
}
