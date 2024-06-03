using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Roles;
using iuca.Application.ViewModels.Users.Roles;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iuca.Application.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IApplicationDbContext _db;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IOrganizationService _organizationService;

        public RoleService(RoleManager<ApplicationRole> roleManager,
            IOrganizationService organizationService,
            IApplicationDbContext db)

        {
            _roleManager = roleManager;
            _organizationService = organizationService;
            _db = db;
        }

        /// <summary>
        /// Get roles grouped by name
        /// </summary>
        /// <returns>List of grouped roles with their ids</returns>
        public List<RoleViewModel> GetRoles() 
        {
            List<RoleViewModel> model = new List<RoleViewModel>();
            var roles = _roleManager.Roles.ToList();
            var splitedRoles = roles
                .Select(x => new { Id = x.Id, Name = x.Name.Split('_')[0], OrganizationId = x.Name.Split('_')[1] }).ToList();

            foreach (var groupedRoles in splitedRoles.GroupBy(x => x.Name)) 
            {
                RoleViewModel roleView = new RoleViewModel();
                roleView.RoleName = groupedRoles.Key;
                roleView.RoleIds = groupedRoles.Select(x => x.Id).ToList();
                roleView.OrganizationIds = groupedRoles.Select(x => x.OrganizationId).ToList();
                model.Add(roleView);
            }

            return model;
        }

        /// <summary>
        /// Get roles grouped by name
        /// </summary>
        /// <returns>List of grouped roles with their ids</returns>
        public List<RoleViewModel> GetRoles(int organizationId)
        {
            List<RoleViewModel> model = new List<RoleViewModel>();
            var roles = _roleManager.Roles.ToList();
            var splitedRoles = roles
                .Select(x => new { Id = x.Id, Name = x.Name.Split('_')[0], OrganizationId = x.Name.Split('_')[1] })
                .Where(x => x.OrganizationId == organizationId.ToString())
                .ToList();

            foreach (var groupedRoles in splitedRoles.GroupBy(x => x.Name))
            {
                RoleViewModel roleView = new RoleViewModel();
                roleView.RoleName = groupedRoles.Key;
                roleView.RoleIds = groupedRoles.Select(x => x.Id).ToList();
                roleView.OrganizationIds = groupedRoles.Select(x => x.OrganizationId).ToList();
                model.Add(roleView);
            }

            return model;
        }

        /// <summary>
        /// Get role by name prefix
        /// </summary>
        /// <param name="roleNamePrefix">Role name prefix</param>
        /// <returns>Role name with role ids</returns>
        public RoleViewModel GetRoleByNamePrefix(string roleNamePrefix)
        {
            var roles = _roleManager.Roles.Where(x => x.Name.StartsWith(roleNamePrefix + "_")).ToList();

            if (roles.Count == 0)
                throw new Exception($"Role with name prefix {roleNamePrefix} not found");

            var splitedRole = roles
                .Select(x => new { Id = x.Id, Name = x.Name.Split('_')[0], OrganizationId = x.Name.Split('_')[1] })
                .GroupBy(x => x.Name)
                .FirstOrDefault();
           

            RoleViewModel roleView = new RoleViewModel();
            roleView.RoleName = splitedRole.Key;
            roleView.RoleIds = splitedRole.Select(x => x.Id).ToList();
            roleView.OrganizationIds = splitedRole.Select(x => x.OrganizationId).ToList();

            return roleView;
        }

        /// <summary>
        /// Create role for each organization
        /// </summary>
        /// <param name="roleName">Role name</param>
        public IdentityResult Create(string roleName)
        {
            if (string.IsNullOrEmpty(roleName) || roleName.Contains("_"))
                throw new ModelValidationException("Role name is incorrect", "ErrorMsg");

            var organizations = _organizationService.GetOrganizations();
            //Check role in each organization
            foreach (var organization in organizations)
            {
                if (_roleManager.RoleExistsAsync($"{roleName}_{organization.Id}").GetAwaiter().GetResult())
                    throw new ModelValidationException("Role already exists", "ErrorMsg");
            }

            IdentityResult result = new IdentityResult();
            using (var transaction = _db.Database.BeginTransaction()) 
            {
                //Create role for each organization
                foreach (var organization in organizations)
                {
                    ApplicationRole applicationRole = new ApplicationRole($"{roleName}_{organization.Id}");
                    result = _roleManager.CreateAsync(applicationRole).GetAwaiter().GetResult();
                    if (!result.Succeeded)
                        break;
                }

                if (result.Succeeded) 
                    transaction.Commit();
                else
                    transaction.Rollback();
            }

            return result;
        }

        /// <summary>
        /// Edit role for each role id
        /// </summary>
        /// <param name="newRoleName">New role name prefix</param>
        /// <param name="roleIds">List of role ids</param>
        /// <returns>Identity result</returns>
        public IdentityResult Edit(string newRoleName, List<string> roleIds) 
        {
            if (string.IsNullOrEmpty(newRoleName) || newRoleName.Contains("_"))
                throw new ModelValidationException("Role name is incorrect", "ErrorMsg");

            IdentityResult result = new IdentityResult();
            using (var transaction = _db.Database.BeginTransaction()) 
            {
                foreach (var roleId in roleIds)
                {
                    ApplicationRole roleDb = _roleManager.FindByIdAsync(roleId).GetAwaiter().GetResult();
                    if (roleDb == null)
                        throw new Exception($"Role with id {roleId} not found");

                    var splitedRoleName = roleDb.Name.Split("_");

                    if (splitedRoleName[0] != newRoleName)
                    {
                        roleDb.Name = newRoleName + "_" + splitedRoleName[1];
                        result = _roleManager.UpdateAsync(roleDb).GetAwaiter().GetResult();
                        if (!result.Succeeded)
                            break;
                    }
                }

                if (result.Succeeded)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
                
            return result;
        }

        /// <summary>
        /// Delete roles by ids
        /// </summary>
        /// <param name="roleIds">Role ids</param>
        /// <returns>Identity result</returns>
        public IdentityResult Delete(List<string> roleIds) 
        {
            IdentityResult result = new IdentityResult();
            using (var transaction = _db.Database.BeginTransaction())
            {
                foreach (var roleId in roleIds)
                {
                    var role = _roleManager.FindByIdAsync(roleId).GetAwaiter().GetResult();
                    if (role != null) 
                    {
                        result = _roleManager.DeleteAsync(role).GetAwaiter().GetResult();
                        if (!result.Succeeded)
                            break;
                    }
                }

                if (result.Succeeded)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
            return result;
        }
    }
}
