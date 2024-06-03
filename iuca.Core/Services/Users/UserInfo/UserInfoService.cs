using iuca.Application.Interfaces.Users;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using iuca.Application.Interfaces.Common;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.ViewModels.Users.UserInfo;
using Microsoft.EntityFrameworkCore;
using iuca.Application.ViewModels.Users;
using iuca.Application.Interfaces.Users.Staff;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Interfaces.Users.Instructors;
using Microsoft.AspNetCore.Mvc.Rendering;
using iuca.Application.Interfaces.Roles;

namespace iuca.Application.Services.Users.UserInfo
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IApplicationDbContext _db;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IOrganizationService _organizationService;
        private readonly IUserTypeOrganizationService _userTypeOrganizationService;
        private readonly IUserBasicInfoService _userBasicInfoService;
        private readonly IStaffBasicInfoService _staffBasicInfoService;
        private readonly IStudentBasicInfoService _studentBasicInfoService;
        private readonly IInstructorBasicInfoService _instructorBasicInfoService;
        private readonly IUserRolesService _userRolesService;

        public UserInfoService(IApplicationDbContext db,
                               ApplicationUserManager<ApplicationUser> userManager,
                               IOrganizationService organizationService,
                               IUserTypeOrganizationService userTypeOrganizationService,
                               IUserBasicInfoService userBasicInfoService,
                               IStaffBasicInfoService staffBasicInfoService,
                               IStudentBasicInfoService studentBasicInfoService,
                               IInstructorBasicInfoService instructorBasicInfoService,
                               IUserRolesService userRolesService)
        {
            _db = db;
            _userManager = userManager;
            _organizationService = organizationService;
            _userTypeOrganizationService = userTypeOrganizationService;
            _userBasicInfoService = userBasicInfoService;
            _staffBasicInfoService = staffBasicInfoService;
            _studentBasicInfoService = studentBasicInfoService;
            _instructorBasicInfoService = instructorBasicInfoService;
            _userRolesService = userRolesService;
        }

        #region Get

        /// <summary>
        /// Get user info list for selected organization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>List of user info models</returns>
        public IEnumerable<UserInfoOrgTypesViewModel> GetUserInfoWithOrgTypesList(int selectedOrganizationId)
        {
            List<UserInfoOrgTypesViewModel> userInfoList = new List<UserInfoOrgTypesViewModel>();

            List<OrganizationDTO> organizationList = _organizationService.GetOrganizations().ToList();
            OrganizationDTO organization = organizationList.FirstOrDefault(x => x.Id == selectedOrganizationId);
            if (organization == null)
                throw new Exception("organization is null");

            var users = _userManager.Users.ToList();

            foreach (var user in users)
            {
                UserInfoOrgTypesViewModel userInfo = new UserInfoOrgTypesViewModel();
                userInfo.ApplicationUser = user;
                userInfo.OrganizationUserTypes = GetOrganizationUserTypes(selectedOrganizationId, user.Id, organizationList);
                //Only main organization is allowed to modify data if IsMainOrganization = true
                userInfo.IsReadOnly = !organization.IsMain && user.IsMainOrganization;
                userInfoList.Add(userInfo);
            }

            return userInfoList;
        }

        /// <summary>
        /// Get list of types associated with organization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization Id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <param name="organizationList">All organization list</param>
        /// <returns>List of associated user types</returns>
        private List<OrganizationUserType> GetOrganizationUserTypes(int selectedOrganizationId, string applicationUserId,
                    List<OrganizationDTO> organizationList)
        {
            List<OrganizationUserType> organizationUserTypes = new List<OrganizationUserType>();

            foreach (OrganizationDTO org in organizationList)
            {
                OrganizationUserType organizationUserType = new OrganizationUserType();
                organizationUserType.Organization = org;
                foreach (enu_UserType userType in Enum.GetValues(typeof(enu_UserType)))
                {
                    SelectedUserType selectedUserType = new SelectedUserType();
                    selectedUserType.UserType = userType;
                    selectedUserType.IsSelected = _userTypeOrganizationService.UserHasType(org.Id, applicationUserId, (int)userType);
                    //Only selected organization changes allowed
                    selectedUserType.IsReadOnly = org.Id != selectedOrganizationId;
                    organizationUserType.SelectedUserTypes.Add(selectedUserType);
                }
                organizationUserTypes.Add(organizationUserType);
            }
            return organizationUserTypes;
        }

        /// <summary>
        /// Get user brief info for selected oranization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <returns>User info model</returns>
        public UserInfoViewModel GetUserInfoByApplicationUserId(int selectedOrganizationId, string applicationUserId)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == applicationUserId);
            if (user == null)
                throw new Exception("User not found");

            return GetUserInfo(selectedOrganizationId, user);
        }

        /// <summary>
        /// Get user brief info for selected oranization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="email">User email</param>
        /// <param name="generateException">If true generates exception when user not found</param>
        /// <returns>User info model</returns>
        public UserInfoViewModel GetUserInfoByEmail(int selectedOrganizationId, string email, bool generateException = true)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Email == email);
            if (user == null) 
            {
                if (generateException)
                    throw new Exception("User not found");
                else 
                    return null;
            }

            return GetUserInfo(selectedOrganizationId, user);
        }

        private UserInfoViewModel GetUserInfo(int organizationId, ApplicationUser user) 
        {
            OrganizationDTO organization = _organizationService.GetOrganizations()
                                            .FirstOrDefault(x => x.Id == organizationId);
            if (organization == null)
                throw new Exception("organization is null");

            UserInfoViewModel userInfo = new UserInfoViewModel();

            userInfo.ApplicationUser = user;
            //Only main organization is allowed to modify data if IsMainOrganization = true
            userInfo.IsReadOnly = !organization.IsMain && user.IsMainOrganization;
            userInfo.IsInstructor = _userTypeOrganizationService.UserHasType(organizationId, user.Id, (int)enu_UserType.Instructor);
            userInfo.IsStudent = _userTypeOrganizationService.UserHasType(organizationId, user.Id, (int)enu_UserType.Student);
            userInfo.IsStaff = _userTypeOrganizationService.UserHasType(organizationId, user.Id, (int)enu_UserType.Staff);

            return userInfo;
        }

        #endregion

        #region Create

        /// <summary>
        /// Create user with informaion and user types
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="userInfo">User info model</param>
        /// <param name="useTransaction">If true save with rollbacks transaction if error is occurs</param>
        public void Create(int selectedOrganizationId, UserInfoViewModel userInfo, bool useTransaction = true) 
        {
            UserInfoModelChecks(userInfo);

            if (useTransaction)
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        CreateUser(selectedOrganizationId, userInfo);
                        transaction.Commit();
                    }
                    catch (ModelValidationException ex)
                    {
                        transaction.Rollback();
                        throw new ModelValidationException(ex.Message, ex.Property);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            else 
            {
                try
                {
                    CreateUser(selectedOrganizationId, userInfo);
                }
                catch (ModelValidationException ex)
                {
                    throw new ModelValidationException(ex.Message, ex.Property);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private void CreateUser(int selectedOrganizationId, UserInfoViewModel userInfo) 
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Email == userInfo.ApplicationUser.Email);
            if (user == null) 
            {
                IdentityResult result = null;
                userInfo.ApplicationUser.UserName = userInfo.ApplicationUser.Email;
                userInfo.ApplicationUser.IsActive = true;
                Task.Run(() => result = _userManager.CreateAsync(userInfo.ApplicationUser).GetAwaiter().GetResult()).Wait();

                if (result == null || !result.Succeeded)
                {
                    throw new Exception("User cannot be created:\n" +
                        (result != null ? string.Join(",", result.Errors.Select(x => x.Description).ToList()) : ""));
                }
            }

            if (userInfo.IsStudent)
                _userTypeOrganizationService.Create(selectedOrganizationId, userInfo.ApplicationUser.Id, (int)enu_UserType.Student);

            if (userInfo.IsInstructor)
                _userTypeOrganizationService.Create(selectedOrganizationId, userInfo.ApplicationUser.Id, (int)enu_UserType.Instructor);

            if (userInfo.IsStaff)
                _userTypeOrganizationService.Create(selectedOrganizationId, userInfo.ApplicationUser.Id, (int)enu_UserType.Staff);

            _db.SaveChanges();
        }

        private void UserInfoModelChecks(UserInfoViewModel userInfo)
        {
            if (userInfo == null)
                throw new Exception("userInfo is null");

            if (userInfo.ApplicationUser == null)
                throw new Exception("ApplicationUser is null");

            EmailChecks(userInfo.ApplicationUser.Email);

            if (!userInfo.IsInstructor && !userInfo.IsStaff && !userInfo.IsStudent)
                throw new ModelValidationException("User must be Instructor, Staff or Student", "ErrorMsg");
        }

        private void EmailChecks(string email)
        {
            if (email == null)
                throw new ModelValidationException("Email is null", "Email");

            if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email))
                throw new ModelValidationException("Email is invalid", "Email");
        }

        #endregion

        #region Edit

        /// <summary>
        /// Edit user, information and user types
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <param name="userInfo">User info model</param>
        /// <param name="useTransaction">If true save with rollbacks transaction if error is occurs</param>
        public void Edit(int selectedOrganizationId, string applicationUserId, UserInfoViewModel userInfo, bool useTransaction = true)
        {
            UserInfoModelChecks(userInfo);

            if (useTransaction)
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        EditUser(selectedOrganizationId, applicationUserId, userInfo);
                        transaction.Commit();
                    }
                    catch (ModelValidationException ex)
                    {
                        transaction.Rollback();
                        throw new ModelValidationException(ex.Message, ex.Property);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            else 
            {
                try
                {
                    EditUser(selectedOrganizationId, applicationUserId, userInfo);
                }
                catch (ModelValidationException ex)
                {
                    throw new ModelValidationException(ex.Message, ex.Property);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private void EditUser(int selectedOrganizationId, string applicationUserId, UserInfoViewModel userInfo) 
        {
            EditApplicationUser(applicationUserId, userInfo.ApplicationUser);
            EditUserType(selectedOrganizationId, userInfo.ApplicationUser.Id, userInfo.IsInstructor, enu_UserType.Instructor);
            EditUserType(selectedOrganizationId, userInfo.ApplicationUser.Id, userInfo.IsStudent, enu_UserType.Student);
            EditUserType(selectedOrganizationId, userInfo.ApplicationUser.Id, userInfo.IsStaff, enu_UserType.Staff);

            _db.SaveChanges();
        }

        private void EditApplicationUser(string applicationUserId, ApplicationUser user) 
        {
            if(user == null)
                throw new Exception("User model is null");

            ApplicationUser dbUser = _userManager.Users.FirstOrDefault(x => x.Id == applicationUserId);
            if(dbUser == null)
                throw new Exception("User not found");

            dbUser.LastNameEng = user.LastNameEng;
            dbUser.FirstNameEng = user.FirstNameEng;
            dbUser.MiddleNameEng = user.MiddleNameEng;
            

            IdentityResult updateResult = null;
            Task.Run(() => updateResult = _userManager.UpdateAsync(dbUser).GetAwaiter().GetResult()).Wait();
            if (updateResult == null || !updateResult.Succeeded)
            {
                throw new ModelValidationException("User cannot be updated:\n" +
                    (updateResult != null ? string.Join(",", updateResult.Errors.Select(x => x.Description).ToList()) : ""), "");
            }

            if (user.Email != dbUser.Email) 
            {
                dbUser.UserName = user.Email;
                var token = _userManager.GenerateChangeEmailTokenAsync(dbUser, user.Email).GetAwaiter().GetResult();
                IdentityResult emailResult = null;
                Task.Run(() => emailResult = _userManager.ChangeEmailAsync(dbUser, user.Email, token).GetAwaiter().GetResult()).Wait();
                if (emailResult == null || !emailResult.Succeeded)
                {
                    throw new ModelValidationException("User cannot be updated:\n" +
                        (emailResult != null ? string.Join(",", emailResult.Errors.Select(x => x.Description).ToList()) : ""), "");
                }
                var logins = _userManager.GetLoginsAsync(dbUser).GetAwaiter().GetResult();
                foreach (var login in logins) 
                {
                    _userManager.RemoveLoginAsync(dbUser, login.LoginProvider, login.ProviderKey).GetAwaiter().GetResult();
                }

                
            }
        }

        /// <summary>
        /// Edit user type and user role if flag is changed
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <param name="userTypeFlag">User type flag</param>
        /// <param name="userType">User type</param>
        private void EditUserType(int selectedOrganizationId, string applicationUserId, bool userTypeFlag, enu_UserType userType) 
        {
            bool hasType = _userTypeOrganizationService.UserHasType(selectedOrganizationId, applicationUserId, (int)userType);
            if (userTypeFlag)
            {
                //If flag is true but user type does not exist
                if (!hasType)
                    _userTypeOrganizationService.Create(selectedOrganizationId, applicationUserId, (int)userType);
            }
            else
            {
                //If flag is false but user type exists
                if (hasType)
                    _userTypeOrganizationService.Delete(selectedOrganizationId, applicationUserId, (int)userType);
            }
        }
        #endregion

        #region Delete

        /// <summary>
        /// Delete user, information and user types
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization</param>
        /// <param name="applicationUserId">Application user id</param>
        public void Delete(int selectedOrganizationId, string applicationUserId) 
        {
            var user = _userManager.Users
                .Include(x => x.UserBasicInfo)
                .FirstOrDefault(x => x.Id == applicationUserId);

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    /*if (user.UserBasicInfo != null)
                        throw new ModelValidationException("User cannot be deleted. Instructor info exists", "ErrorMsg");*/

                    _userManager.DeleteAsync(user).GetAwaiter().GetResult();

                    _db.SaveChanges();
                    transaction.Commit();
                }
                catch (ModelValidationException ex)
                {
                    transaction.Rollback();
                    throw new ModelValidationException(ex.Message, ex.Property);
                }
                catch (Exception ex) 
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        #endregion

        /// <summary>
        /// Get user account info by user basic info id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <returns>Account view model</returns>
        public AccountInfoViewModel GetUserAccountInfo(int organizationId, string applicationUserId) 
        {
            AccountInfoViewModel accountInfo = new AccountInfoViewModel();
            accountInfo.UserInfo = GetUserInfoByApplicationUserId(organizationId, applicationUserId);

            accountInfo.UserBasicInfo = _userBasicInfoService.GetUserFullInfo(applicationUserId, false);
            if (accountInfo.UserInfo.IsStaff) 
            {
                accountInfo.StaffBasicInfo = _staffBasicInfoService.GetStaffFullInfo(applicationUserId, false);
            }

            if (accountInfo.UserInfo.IsInstructor)
            {
                accountInfo.InstructorBasicInfo = _instructorBasicInfoService.GetInstructorFullInfo(applicationUserId, false);
                if (accountInfo.InstructorBasicInfo != null && accountInfo.InstructorBasicInfo.InstructorOrgInfo != null)
                    accountInfo.InstructorOrgInfo = accountInfo.InstructorBasicInfo.InstructorOrgInfo
                        .FirstOrDefault(x => x.OrganizationId == organizationId);
            }

            if (accountInfo.UserInfo.IsStudent)
            {
                accountInfo.StudentBasicInfo = _studentBasicInfoService.GetStudentFullInfo(applicationUserId, false);
                if (accountInfo.StudentBasicInfo != null && accountInfo.StudentBasicInfo.StudentOrgInfo != null)
                    accountInfo.StudentOrgInfo = accountInfo.StudentBasicInfo.StudentOrgInfo
                        .FirstOrDefault(x => x.OrganizationId == organizationId); ;
            }
            
            return accountInfo;
        }

        /// <summary>
        /// Get user SelectList
        /// </summary>
        /// <param name="selectedOrganizationId">Id of selected organization</param>
        /// <param name="role">Filter users by role</param>
        /// <param name="userId">Selected user id</param>
        /// <returns>SelectList of users</returns>
        public List<SelectListItem> GetUserSelectList(int selectedOrganizationId, enu_Role? role, string userId = null)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            if (role != null)
                users = _userRolesService.GetUserListByRole(selectedOrganizationId, role.Value).ToList();
            else 
                users = _userManager.Users.ToList();
            
            return new SelectList(users.OrderBy(x => x.FullNameEng), "Id", "FullNameEng", userId).ToList();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
