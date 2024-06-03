using iuca.Application.Enums;
using iuca.Application.ViewModels;
using iuca.Application.ViewModels.Users;
using iuca.Application.ViewModels.Users.UserInfo;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Users.UserInfo
{
    public interface IUserInfoService
    {
        /// <summary>
        /// Get user info list for selected organization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>List of user info models</returns>
        public IEnumerable<UserInfoOrgTypesViewModel> GetUserInfoWithOrgTypesList(int selectedOrganizationId);

        /// <summary>
        /// Get user brief info for selected oranization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <returns>User info model</returns>
        UserInfoViewModel GetUserInfoByApplicationUserId(int selectedOrganizationId, string applicationUserId);

        /// <summary>
        /// Get user brief info for selected oranization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="email">User email</param>
        /// <param name="generateException">If true generates exception when user not found</param>
        /// <returns>User info model</returns>
        UserInfoViewModel GetUserInfoByEmail(int selectedOrganizationId, string email, bool generateException = true);

        /// <summary>
        /// Create user with informaion and user types
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="userInfo">User info model</param>
        /// <param name="useTransaction">If true save with rollbacks transaction if error is occurs</param>
        void Create(int selectedOrganizationId, UserInfoViewModel userInfo, bool useTransaction = true);

        /// <summary>
        /// Edit user, information and user types
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <param name="userInfo">User info model</param>
        /// <param name="useTransaction">If true save with rollbacks transaction if error is occurs</param>
        void Edit(int selectedOrganizationId, string applicationUserId, UserInfoViewModel userInfo, bool useTransaction = true);

        /// <summary>
        /// Delete user, information and user types
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization</param>
        /// <param name="applicationUserId">Application user id</param>
        void Delete(int selectedOrganizationId, string applicationUserId);

        /// <summary>
        /// Get user account info by user basic info id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <returns>Account view model</returns>
        AccountInfoViewModel GetUserAccountInfo(int organizationId, string applicationUserId);

        /// <summary>
        /// Get user SelectList
        /// </summary>
        /// <param name="selectedOrganizationId">Id of selected organization</param>
        /// <param name="role">Filter users by role</param>
        /// <param name="userId">Selected user id</param>
        /// <returns>SelectList of users</returns>
        List<SelectListItem> GetUserSelectList(int selectedOrganizationId, enu_Role? role, string userId = null);
        
        void Dispose();
    }
}
