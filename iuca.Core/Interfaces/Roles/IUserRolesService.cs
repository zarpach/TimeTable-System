using iuca.Application.DTO;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.ViewModels.Users.Roles;
using iuca.Infrastructure.Identity.Entities;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Roles
{
    public interface IUserRolesService
    {
        /// <summary>
        /// Get list of users for selected organization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>List of users</returns>
        IEnumerable<ApplicationUser> GetUserList(int selectedOrganizationId);

        /// <summary>
        /// Get user brief info and organization with associated roles
        /// </summary>
        /// <param name="userId">Application user id</param>
        /// <returns>Manage role model</returns>
        UserRolesViewModel GetUserRolesInfo(int selectedOrganizationId, string userId);

        /// <summary>
        /// Get list of users by role
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="role">Role</param>
        /// <returns>List of users</returns>
        IEnumerable<ApplicationUser> GetUserListByRole(int organizationId, enu_Role role);

        /// <summary>
        /// Get list of users by role full name
        /// </summary>
        /// <param name="roleFullName">Role full name</param>
        /// <returns>List of users</returns>
        IEnumerable<ApplicationUser> GetUserListByRole(string roleFullName);

        /// <summary>
        /// If the role is of the user type
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns></returns>
        bool IsUserType(string roleName);

        /// <summary>
        /// Add role to user with organization id postfix
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="role">Role name</param>
        void AddToRole(string userId, int organizationId, enu_Role role);

        /// <summary>
        /// Add role to user with organization id postfix
        /// </summary>
        /// <param name="user">Application user</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="role">Role name</param>
        void AddToRole(ApplicationUser user, int organizationId, enu_Role role);

        /// <summary>
        /// Add role to user 
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="roleName">Role name</param>
        void AddToRole(string userId, string roleName);

        /// <summary>
        /// Add role to user 
        /// </summary>
        /// <param name="user">Application user</param>
        /// <param name="roleName">Role name</param>
        void AddToRole(ApplicationUser user, string roleName);

        /// <summary>
        /// Remove role from userwith organization id postfix
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="role">Role name</param>
        void RemoveFromRole(string userId, int organizationId, enu_Role role);


        /// <summary>
        /// Remove role from user with organization id postfix
        /// </summary>
        /// <param name="user">Application user</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="role">Role name</param>
        void RemoveFromRole(ApplicationUser user, int organizationId, enu_Role role);

        /// <summary>
        /// Remove role from user 
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="roleName">Role name</param>
        void RemoveFromRole(string userId, string roleName);

        /// <summary>
        /// Remove role from user 
        /// </summary>
        /// <param name="user">Application user</param>
        /// <param name="roleName">Role name</param>
        void RemoveFromRole(ApplicationUser user, string roleName);

        /// <summary>
        /// Check if user has role
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="role">Role</param>
        /// <returns>True if user in role</returns>
        bool IsUserInRole(string userId, int organizationId, enu_Role role);

        /// <summary>
        /// Check if user has role
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="roleFullName">Role name with postfix</param>
        /// <returns>True if user in role</returns>
        bool IsUserInRole(string userId, string roleFullName);

        /// <summary>
        /// Check if user has role
        /// </summary>
        /// <param name="user">Application user</param>
        /// <param name="roleFullName">Role name with postfix</param>
        /// <returns>True if user in role</returns>
        bool IsUserInRole(ApplicationUser user, string roleFullName);

        void Dispose();
    }
}
