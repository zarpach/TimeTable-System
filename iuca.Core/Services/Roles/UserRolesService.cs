using iuca.Application.DTO.Common;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Roles;
using iuca.Application.ViewModels.Users.Roles;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iuca.Application.Services.Roles
{
    public class UserRolesService : IUserRolesService
    {
        private readonly IApplicationDbContext _db;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IRoleService _roleService;
        private readonly IOrganizationService _organizationService;

        public UserRolesService(IApplicationDbContext db, 
            ApplicationUserManager<ApplicationUser> userManager,
            IRoleService roleService,
            IOrganizationService organizationService)
        {
            _db = db;
            _userManager = userManager;
            _roleService = roleService;
            _organizationService = organizationService;
        }

        /// <summary>
        /// Get list of users for selected organization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>List of users</returns>
        public IEnumerable<ApplicationUser> GetUserList(int selectedOrganizationId) 
        {
            List<UserRolesBriefViewModel> userRoles = new List<UserRolesBriefViewModel>();
            List<string> applicationUserIds = _db.UserTypeOrganizations.Where(x => x.OrganizationId == selectedOrganizationId)
                                                        .Select(x => x.ApplicationUserId).Distinct().ToList();

            return _userManager.Users.Where(x => applicationUserIds.Contains(x.Id)).ToList();
        }

        /// <summary>
        /// Get list of users by role
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="role">Role</param>
        /// <returns>List of users</returns>
        public IEnumerable<ApplicationUser> GetUserListByRole(int organizationId, enu_Role role)
        {
            return GetUserListByRole($"{role.ToString()}_{organizationId}");
        }

        /// <summary>
        /// Get list of users by role full name
        /// </summary>
        /// <param name="roleFullName">Role full name</param>
        /// <returns>List of users</returns>
        public IEnumerable<ApplicationUser> GetUserListByRole(string roleFullName)
        {
            return _userManager.GetUsersInRoleAsync(roleFullName).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get user brief info and organization with associated roles
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <returns>Manage role model</returns>
        public UserRolesViewModel GetUserRolesInfo(int selectedOrganizationId, string applicationUserId) 
        {
            UserRolesViewModel userRoles = new UserRolesViewModel();
            ApplicationUser applicationUser = _userManager.Users.FirstOrDefault(x => x.Id == applicationUserId);
            if (applicationUser == null)
                throw new Exception("User not found");

            userRoles.ApplicationUserId = applicationUser.Id;
            userRoles.FullNameEng = applicationUser.FullNameEng;
            userRoles.Email = applicationUser.Email;

            OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
            if (organization == null)
                throw new Exception($"Organization with id {selectedOrganizationId} not found");

            userRoles.Organization = organization;

            foreach (var role in _roleService.GetRoles(selectedOrganizationId))
            {
                SelectedRole selectedRole = new SelectedRole();
                selectedRole.RoleName = role.RoleName;
                selectedRole.IsSelected = IsUserInRole(applicationUser, $"{role.RoleName}_{selectedOrganizationId}");
                selectedRole.IsReadonly = IsUserType(role.RoleName);
                userRoles.SelectedRoles.Add(selectedRole);
            }

            return userRoles;
        }

        /// <summary>
        /// If the role is of the user type
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns></returns>
        public bool IsUserType(string roleName) 
        {
            //Can be changed from switching user type only
            return roleName == enu_Role.Staff.ToString() || roleName == enu_Role.Student.ToString()
                || roleName == enu_Role.Instructor.ToString();
        }

        /// <summary>
        /// Add role to user with organization id postfix
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="role">Role name</param>
        public void AddToRole(string userId, int organizationId, enu_Role role)
        {
            AddToRole(userId, role.ToString() + "_" + organizationId);
        }

        /// <summary>
        /// Add role to user with organization id postfix
        /// </summary>
        /// <param name="user">Application user</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="role">Role name</param>
        public void AddToRole(ApplicationUser user, int organizationId, enu_Role role)
        {
            AddToRole(user, role.ToString() + "_" + organizationId);
        }

        /// <summary>
        /// Add role to user 
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="roleName">Role name</param>
        public void AddToRole(string userId, string roleName) 
        {
            ApplicationUser user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
                throw new Exception("User not found");
            
            AddToRole(user, roleName);
        }

        /// <summary>
        /// Add role to user 
        /// </summary>
        /// <param name="user">Application user</param>
        /// <param name="roleName">Role name</param>
        public void AddToRole(ApplicationUser user, string roleName)
        {
            if (!IsRoleMatchPattern(roleName))
                throw new Exception("Role is not matched to pattern: {role name}_{organization id}");

            Task.Run(() => _userManager.AddToRoleAsync(user, roleName).GetAwaiter().GetResult()).Wait();
        }

        /// <summary>
        /// Remove role from userwith organization id postfix
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="role">Role name</param>
        public void RemoveFromRole(string userId, int organizationId, enu_Role role)
        {
            RemoveFromRole(userId, role.ToString() + "_" + organizationId);
        }

        /// <summary>
        /// Remove role from user with organization id postfix
        /// </summary>
        /// <param name="user">Application user</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="role">Role name</param>
        public void RemoveFromRole(ApplicationUser user, int organizationId, enu_Role role)
        {
            RemoveFromRole(user, role.ToString() + "_" + organizationId);
        }

        /// <summary>
        /// Remove role from user 
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="roleName">Role name</param>
        public void RemoveFromRole(string userId, string roleName)
        {
            ApplicationUser user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
                throw new Exception("User not found");

            RemoveFromRole(user, roleName);
        }

        /// <summary>
        /// Remove role from user 
        /// </summary>
        /// <param name="user">Application user</param>
        /// <param name="roleName">Role name</param>
        public void RemoveFromRole(ApplicationUser user, string roleName)
        {
            if (!IsRoleMatchPattern(roleName))
                throw new Exception("Role is not matched to pattern: {role name}_{organization id}");

            _userManager.RemoveFromRoleAsync(user, roleName).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Check if user has role
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="role">Role</param>
        /// <returns>True if user in role</returns>
        public bool IsUserInRole(string userId, int organizationId, enu_Role role)
        {
            return IsUserInRole(userId, $"{role.ToString()}_{organizationId}");
        }

        /// <summary>
        /// Check if user has role
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="roleFullName">Role name with postfix</param>
        /// <returns>True if user in role</returns>
        public bool IsUserInRole(string userId, string roleFullName)
        {
            ApplicationUser user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
                throw new Exception("User not found");

            return IsUserInRole(user, roleFullName);
        }

        /// <summary>
        /// Check if user has role
        /// </summary>
        /// <param name="user">Application user</param>
        /// <param name="roleFullName">Role name with postfix</param>
        /// <returns>True if user in role</returns>
        public bool IsUserInRole(ApplicationUser user, string roleFullName)
        {
            if (!IsRoleMatchPattern(roleFullName))
                throw new Exception("Role is not matched to pattern: {role name}_{organization id}");

            var userRoles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();
            
            return userRoles.Contains(roleFullName);
        }

        private bool IsRoleMatchPattern(string roleName) 
        {
            Regex rolePattern = new Regex(@"[a-z]*_[0-9]*");
            return rolePattern.IsMatch(roleName);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
