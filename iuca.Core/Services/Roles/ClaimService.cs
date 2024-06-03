using iuca.Application.Constants;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Roles;
using iuca.Application.ViewModels.Users.Roles;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Roles
{
    public class ClaimService : IClaimService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ClaimService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        /// <summary>
        /// Get permissions from policy
        /// </summary>
        /// <param name="policy">Policy</param>
        /// <returns>List of claims</returns>
        public List<RoleClaimsViewModel> GetPermissions(Type policy)
        {
            List<RoleClaimsViewModel> allPermissions = new List<RoleClaimsViewModel>();
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);
            
            foreach (FieldInfo fi in fields)
            {
                allPermissions.Add(new RoleClaimsViewModel { ModuleName = policy.Name, Value = fi.GetValue(null).ToString(), Type = "Permissions" });
            }

            return allPermissions;
        }

        /// <summary>
        /// Add permission claims for module
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="role">Role</param>
        /// <param name="module">Permission value</param>
        public void AddPermissionClaim(int organizationId, enu_Role role, string permissionValue)
        {
            AddPermissionClaim(role.ToString() + "_" + organizationId, permissionValue);
        }

        /// <summary>
        /// Add permission claims
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <param name="module">Permission value</param>
        public void AddPermissionClaim(string roleName, string permissionValue)
        {
            var role = Task.Run(() => _roleManager.FindByNameAsync(roleName)).GetAwaiter().GetResult();
            AddPermissionClaim(role, permissionValue);
        }

        /// <summary>
        /// Add permission claim
        /// </summary>
        /// <param name="role">Role</param>
        /// <param name="permissionValue">Permission value</param>
        public void AddPermissionClaim(ApplicationRole role, string permissionValue)
        {
            var allClaims = Task.Run(() => _roleManager.GetClaimsAsync(role)).GetAwaiter().GetResult();
            if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permissionValue))
            {
                _roleManager.AddClaimAsync(role, new Claim("Permission", permissionValue)).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Add permission claims for module
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="role">Role</param>
        /// <param name="module">Permission module</param>
        public void AddPermissionClaimForModule(int organizationId, enu_Role role, string module)
        {
            AddPermissionClaimForModule(role.ToString() + "_" + organizationId, module);
        }

        /// <summary>
        /// Add permission claims for module
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <param name="module">Permission module</param>
        public void AddPermissionClaimForModule(string roleName, string module)
        {
            var role = Task.Run(() => _roleManager.FindByNameAsync(roleName)).GetAwaiter().GetResult();
            AddPermissionClaimForModule(role, module);
        }

        /// <summary>
        /// Add permission claims for module
        /// </summary>
        /// <param name="role">Role</param>
        /// <param name="module">Permission module</param>
        public void AddPermissionClaimForModule(ApplicationRole role, string module)
        {
            var allClaims = Task.Run(() => _roleManager.GetClaimsAsync(role)).GetAwaiter().GetResult();
            var allPermissions = Permissions.GeneratePermissionsForModule(module);
            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    _roleManager.AddClaimAsync(role, new Claim("Permission", permission)).GetAwaiter().GetResult();
                }
            }
        }
    }
}
