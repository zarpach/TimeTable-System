using iuca.Application.Enums;
using iuca.Application.ViewModels.Users.Roles;
using iuca.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Roles
{
    public interface IClaimService
    {

        /// <summary>
        /// Get permissions from policy
        /// </summary>
        /// <param name="policy">Policy</param>
        /// <returns>List of claims</returns>
        List<RoleClaimsViewModel> GetPermissions(Type policy);

        /// <summary>
        /// Add permission claims
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="role">Role</param>
        /// <param name="module">Permission value</param>
        void AddPermissionClaim(int organizationId, enu_Role role, string permissionValue);

        /// <summary>
        /// Add permission claims
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <param name="module">Permission value</param>
        void AddPermissionClaim(string roleName, string permissionValue);

        /// <summary>
        /// Add permission claims
        /// </summary>
        /// <param name="role">Role</param>
        /// <param name="module">Permission value</param>
        void AddPermissionClaim(ApplicationRole role, string permissionValue);

        /// <summary>
        /// Add permission claims for module
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="role">Role</param>
        /// <param name="module">Permission module</param>
        void AddPermissionClaimForModule(int organizationId, enu_Role role, string module);

        /// <summary>
        /// Add permission claims for module
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <param name="module">Permission module</param>
        void AddPermissionClaimForModule(string roleName, string module);

        /// <summary>
        /// Add permission claims for module
        /// </summary>
        /// <param name="role">Role</param>
        /// <param name="module">Permission module</param>
        void AddPermissionClaimForModule(ApplicationRole role, string module);
    }
}
